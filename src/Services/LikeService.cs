using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Data;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;

namespace Melodify.Services
{
    public class LikeService : ILikeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LikeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task LikeTrackAsync(string userId, int trackId)
        {
            var alreadyLiked = await _context.LikedTracks.AnyAsync(lt => lt.UserId == userId && lt.TrackId == trackId);
            if (!alreadyLiked)
            {
                var liked = new LikedTrack
                {
                    UserId = userId,
                    TrackId = trackId,
                    LikedAt = DateTime.UtcNow
                };
                await _context.LikedTracks.AddAsync(liked);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnlikeTrackAsync(string userId, int trackId)
        {
            var liked = await _context.LikedTracks.FirstOrDefaultAsync(lt => lt.UserId == userId && lt.TrackId == trackId);
            if (liked != null)
            {
                _context.LikedTracks.Remove(liked);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TrackDto>> GetLikedTracksAsync(string userId)
        {
            var tracks = await _context.LikedTracks
                .Include(lt => lt.Track)
                    .ThenInclude(t => t.Artist)
                .Include(lt => lt.Track)
                    .ThenInclude(t => t.Album)
                .Where(lt => lt.UserId == userId)
                .OrderByDescending(lt => lt.LikedAt)
                .Select(lt => lt.Track)
                .ToListAsync();

            var dtos = _mapper.Map<IEnumerable<TrackDto>>(tracks).ToList();
            foreach (var dto in dtos)
            {
                dto.IsLiked = true;
            }
            return dtos;
        }
    }
}
