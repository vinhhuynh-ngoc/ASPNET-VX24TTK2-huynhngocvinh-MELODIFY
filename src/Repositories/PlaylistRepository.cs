using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Melodify.Data;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly AppDbContext _context;

        public PlaylistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(string userId)
        {
            return await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }

        public async Task<Playlist?> GetByIdWithTracksAsync(int id)
        {
            return await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track)
                        .ThenInclude(t => t.Artist)
                .FirstOrDefaultAsync(p => p.PlaylistId == id);
        }

        public async Task AddAsync(Playlist playlist)
        {
            await _context.Playlists.AddAsync(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }

        public async Task AddTrackToPlaylistAsync(PlaylistTrack playlistTrack)
        {
            await _context.PlaylistTracks.AddAsync(playlistTrack);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId)
        {
            var pt = await _context.PlaylistTracks
                .FirstOrDefaultAsync(x => x.PlaylistId == playlistId && x.TrackId == trackId);
            if (pt != null)
            {
                _context.PlaylistTracks.Remove(pt);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTrackInPlaylistAsync(int playlistId, int trackId)
        {
            return await _context.PlaylistTracks
                .AnyAsync(x => x.PlaylistId == playlistId && x.TrackId == trackId);
        }
    }
}
