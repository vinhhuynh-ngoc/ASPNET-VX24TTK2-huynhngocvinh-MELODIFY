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
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public PlaylistService(IPlaylistRepository playlistRepository, IMapper mapper, AppDbContext context)
        {
            _playlistRepository = playlistRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<PlaylistDto>> GetUserPlaylistsAsync(string userId)
        {
            var playlists = await _playlistRepository.GetUserPlaylistsAsync(userId);
            return _mapper.Map<IEnumerable<PlaylistDto>>(playlists);
        }

        public async Task<PlaylistDto?> GetPlaylistByIdAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist == null) return null;
            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task<PlaylistDto?> GetPlaylistDetailsAsync(int id, string userId)
        {
            var playlist = await _playlistRepository.GetByIdWithTracksAsync(id);
            if (playlist == null) return null;

            var dto = _mapper.Map<PlaylistDto>(playlist);
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

        public async Task<PlaylistDto> CreatePlaylistAsync(string userId, string name)
        {
            var playlist = new Playlist
            {
                UserId = userId,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };
            await _playlistRepository.AddAsync(playlist);
            return _mapper.Map<PlaylistDto>(playlist);
        }

        public async Task DeletePlaylistAsync(int id)
        {
            var playlist = await _playlistRepository.GetByIdAsync(id);
            if (playlist != null)
            {
                await _playlistRepository.DeleteAsync(playlist);
            }
        }

        public async Task AddTrackToPlaylistAsync(int playlistId, int trackId)
        {
            var exists = await _playlistRepository.IsTrackInPlaylistAsync(playlistId, trackId);
            if (!exists)
            {
                var pt = new PlaylistTrack
                {
                    PlaylistId = playlistId,
                    TrackId = trackId
                };
                await _playlistRepository.AddTrackToPlaylistAsync(pt);
            }
        }

        public async Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId)
        {
            await _playlistRepository.RemoveTrackFromPlaylistAsync(playlistId, trackId);
        }
    }
}
