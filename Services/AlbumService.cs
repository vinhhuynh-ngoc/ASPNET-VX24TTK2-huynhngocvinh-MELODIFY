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
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public AlbumService(IAlbumRepository albumRepository, IMapper mapper, AppDbContext context)
        {
            _albumRepository = albumRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync()
        {
            var albums = await _albumRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AlbumDto>>(albums);
        }

        public async Task<IEnumerable<AlbumDto>> GetRecentAlbumsAsync(int count)
        {
            var albums = await _albumRepository.GetRecentAlbumsAsync(count);
            return _mapper.Map<IEnumerable<AlbumDto>>(albums);
        }

        public async Task<AlbumDto?> GetAlbumByIdAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);
            if (album == null) return null;
            return _mapper.Map<AlbumDto>(album);
        }

        public async Task<AlbumDto?> GetAlbumDetailsAsync(int id, string userId)
        {
            var album = await _albumRepository.GetByIdWithTracksAsync(id);
            if (album == null) return null;

            var dto = _mapper.Map<AlbumDto>(album);
            if (dto.Tracks != null && dto.Tracks.Any() && !string.IsNullOrEmpty(userId))
            {
                var trackIds = dto.Tracks.Select(t => t.TrackId).ToList();
                var likedTrackIds = await _context.LikedTracks
                    .Where(lt => lt.UserId == userId && trackIds.Contains(lt.TrackId))
                    .Select(lt => lt.TrackId)
                    .ToListAsync();

                foreach (var track in dto.Tracks)
                {
                    track.IsLiked = likedTrackIds.Contains(track.TrackId);
                }
            }
            return dto;
        }

        public async Task<PagedResult<AlbumDto>> GetPagedAdminAlbumsAsync(int page, int pageSize, string? search)
        {
            var result = await _albumRepository.GetPagedAdminAsync(page, pageSize, search);
            var dtos = _mapper.Map<IEnumerable<AlbumDto>>(result.Items).ToList();
            
            return new PagedResult<AlbumDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddAlbumAsync(Album album)
        {
            await _albumRepository.AddAsync(album);
        }

        public async Task UpdateAlbumAsync(Album album)
        {
            await _albumRepository.UpdateAsync(album);
        }

        public async Task DeleteAlbumAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);
            if (album != null)
            {
                await _albumRepository.DeleteAsync(album);
            }
        }
    }
}
