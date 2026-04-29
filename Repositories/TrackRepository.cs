using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Melodify.Data;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public class TrackRepository : ITrackRepository
    {
        private readonly AppDbContext _context;

        public TrackRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            return await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .ToListAsync();
        }

        public async Task<Track?> GetByIdAsync(int id)
        {
            return await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .FirstOrDefaultAsync(t => t.TrackId == id);
        }

        public async Task<IEnumerable<Track>> GetFeaturedTracksAsync(int count)
        {
            return await _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .OrderByDescending(t => t.PlayCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Track> Tracks, int TotalCount)> SearchAsync(string query, int page = 1, int pageSize = 20)
        {
            var queryable = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                queryable = queryable.Where(t => t.Title.Contains(query) || 
                                            (t.Artist != null && t.Artist.Name.Contains(query)) || 
                                            (t.Genre != null && t.Genre.Contains(query)));
            }

            var totalCount = await queryable.CountAsync();

            var tracks = await queryable
                .OrderByDescending(t => t.PlayCount)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tracks, totalCount);
        }

        public async Task<(IEnumerable<Track> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null, int? artistId = null, string? sortBy = null)
        {
            var queryable = _context.Tracks
                .Include(t => t.Artist)
                .Include(t => t.Album)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(t => t.Title.Contains(search) || 
                                            (t.Artist != null && t.Artist.Name.Contains(search)) || 
                                            (t.Genre != null && t.Genre.Contains(search)));
            }

            if (artistId.HasValue && artistId.Value > 0)
            {
                queryable = queryable.Where(t => t.ArtistId == artistId.Value);
            }

            queryable = sortBy switch
            {
                "playCount_desc" => queryable.OrderByDescending(t => t.PlayCount),
                "playCount_asc" => queryable.OrderBy(t => t.PlayCount),
                "title_asc" => queryable.OrderBy(t => t.Title),
                "title_desc" => queryable.OrderByDescending(t => t.Title),
                "duration_desc" => queryable.OrderByDescending(t => t.Duration),
                "duration_asc" => queryable.OrderBy(t => t.Duration),
                _ => queryable.OrderByDescending(t => t.TrackId)
            };

            var totalCount = await queryable.CountAsync();
            var items = await queryable.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Track track)
        {
            await _context.Tracks.AddAsync(track);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Track track)
        {
            _context.Tracks.Update(track);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Track track)
        {
            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync();
        }
    }
}
