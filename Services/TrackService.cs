using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Data;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;
using Melodify.Repositories;

namespace Melodify.Services
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public TrackService(ITrackRepository trackRepository, IMapper mapper, AppDbContext context)
        {
            _trackRepository = trackRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<TrackDto>> GetAllTracksAsync(string userId)
        {
            var tracks = await _trackRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<TrackDto>>(tracks).ToList();
            await PopulateLikedStatusAsync(dtos, userId);
            return dtos;
        }

        public async Task<TrackDto?> GetTrackByIdAsync(int id, string userId)
        {
            var track = await _trackRepository.GetByIdAsync(id);
            if (track == null) return null;
            var dto = _mapper.Map<TrackDto>(track);
            dto.IsLiked = await _context.LikedTracks.AnyAsync(lt => lt.UserId == userId && lt.TrackId == id);
            return dto;
        }

        public async Task<IEnumerable<TrackDto>> GetFeaturedTracksAsync(int count, string userId)
        {
            var tracks = await _trackRepository.GetFeaturedTracksAsync(count);
            var dtos = _mapper.Map<IEnumerable<TrackDto>>(tracks).ToList();
            await PopulateLikedStatusAsync(dtos, userId);
            return dtos;
        }

        public async Task<PagedResult<TrackDto>> SearchTracksAsync(string query, string userId, int page = 1, int pageSize = 20)
        {
            var result = await _trackRepository.SearchAsync(query, page, pageSize);
            var dtos = _mapper.Map<IEnumerable<TrackDto>>(result.Tracks).ToList();
            await PopulateLikedStatusAsync(dtos, userId);
            
            return new PagedResult<TrackDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<TrackDto>> GetPagedAdminTracksAsync(int page, int pageSize, string? search, int? artistId, string? sortBy)
        {
            var result = await _trackRepository.GetPagedAdminAsync(page, pageSize, search, artistId, sortBy);
            var dtos = _mapper.Map<IEnumerable<TrackDto>>(result.Items).ToList();
            
            return new PagedResult<TrackDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task IncrementPlayCountAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id);
            if (track != null)
            {
                track.PlayCount++;
                await _trackRepository.UpdateAsync(track);
            }
        }

        public async Task AddTrackAsync(Track track)
        {
            await _trackRepository.AddAsync(track);
        }

        public async Task UpdateTrackAsync(Track track)
        {
            await _trackRepository.UpdateAsync(track);
        }

        public async Task DeleteTrackAsync(int id)
        {
            var track = await _trackRepository.GetByIdAsync(id);
            if (track != null)
            {
                await _trackRepository.DeleteAsync(track);
            }
        }

        private async Task PopulateLikedStatusAsync(List<TrackDto> dtos, string userId)
        {
            if (string.IsNullOrEmpty(userId) || !dtos.Any()) return;

            var trackIds = dtos.Select(d => d.TrackId).ToList();
            var likedTrackIds = await _context.LikedTracks
                .Where(lt => lt.UserId == userId && trackIds.Contains(lt.TrackId))
                .Select(lt => lt.TrackId)
                .ToListAsync();

            foreach (var dto in dtos)
            {
                dto.IsLiked = likedTrackIds.Contains(dto.TrackId);
            }
        }
    }
}
