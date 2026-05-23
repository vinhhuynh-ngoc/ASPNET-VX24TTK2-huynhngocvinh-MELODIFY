using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Data;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;
using Melodify.Repositories;

namespace Melodify.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public ArtistService(IArtistRepository artistRepository, IMapper mapper, AppDbContext context)
        {
            _artistRepository = artistRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ArtistDto>> GetAllArtistsAsync(string userId)
        {
            var artists = await _artistRepository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<ArtistDto>>(artists).ToList();
            await PopulateFollowStatusAsync(dtos, userId);
            return dtos;
        }

        public async Task<IEnumerable<ArtistDto>> GetSuggestedArtistsAsync(int count, string userId)
        {
            var artists = await _artistRepository.GetSuggestedArtistsAsync(count);
            var dtos = _mapper.Map<IEnumerable<ArtistDto>>(artists).ToList();
            await PopulateFollowStatusAsync(dtos, userId);
            return dtos;
        }

        public async Task<ArtistDto?> GetArtistByIdAsync(int id, string userId)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            if (artist == null) return null;
            var dto = _mapper.Map<ArtistDto>(artist);
            dto.IsFollowed = await _context.FollowedArtists.AnyAsync(fa => fa.UserId == userId && fa.ArtistId == id);
            return dto;
        }

        public async Task<ArtistDto?> GetArtistDetailsAsync(int id, string userId)
        {
            var artist = await _artistRepository.GetByIdWithDetailsAsync(id);
            if (artist == null) return null;
            var dto = _mapper.Map<ArtistDto>(artist);
            dto.IsFollowed = await _context.FollowedArtists.AnyAsync(fa => fa.UserId == userId && fa.ArtistId == id);
            return dto;
        }

        public async Task<PagedResult<ArtistDto>> GetPagedAdminArtistsAsync(int page, int pageSize, string? search)
        {
            var result = await _artistRepository.GetPagedAdminAsync(page, pageSize, search);
            var dtos = _mapper.Map<IEnumerable<ArtistDto>>(result.Items).ToList();
            
            return new PagedResult<ArtistDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task AddArtistAsync(Artist artist)
        {
            await _artistRepository.AddAsync(artist);
        }

        public async Task UpdateArtistAsync(Artist artist)
        {
            await _artistRepository.UpdateAsync(artist);
        }

        public async Task DeleteArtistAsync(int id)
        {
            var artist = await _artistRepository.GetByIdAsync(id);
            if (artist != null)
            {
                await _artistRepository.DeleteAsync(artist);
            }
        }

        public async Task FollowArtistAsync(string userId, int artistId)
        {
            var alreadyFollowed = await _context.FollowedArtists.AnyAsync(fa => fa.UserId == userId && fa.ArtistId == artistId);
            if (!alreadyFollowed)
            {
                var follow = new FollowArtist
                {
                    UserId = userId,
                    ArtistId = artistId,
                    FollowedAt = DateTime.UtcNow
                };
                await _context.FollowedArtists.AddAsync(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnfollowArtistAsync(string userId, int artistId)
        {
            var follow = await _context.FollowedArtists.FirstOrDefaultAsync(fa => fa.UserId == userId && fa.ArtistId == artistId);
            if (follow != null)
            {
                _context.FollowedArtists.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        private async Task PopulateFollowStatusAsync(List<ArtistDto> dtos, string userId)
        {
            if (string.IsNullOrEmpty(userId) || !dtos.Any()) return;

            var artistIds = dtos.Select(d => d.ArtistId).ToList();
            var followedArtistIds = await _context.FollowedArtists
                .Where(fa => fa.UserId == userId && artistIds.Contains(fa.ArtistId))
                .Select(fa => fa.ArtistId)
                .ToListAsync();

            foreach (var dto in dtos)
            {
                dto.IsFollowed = followedArtistIds.Contains(dto.ArtistId);
            }
        }
    }
}
