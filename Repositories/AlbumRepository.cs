using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Melodify.Data;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly AppDbContext _context;

        public AlbumRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Album>> GetAllAsync()
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .ToListAsync();
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);
        }

        public async Task<Album?> GetByIdWithTracksAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Tracks)
                    .ThenInclude(t => t.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);
        }

        public async Task<IEnumerable<Album>> GetFeaturedAlbumsAsync(int count)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .OrderByDescending(a => a.AlbumId) // Just using ID desc as a proxy for now
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Album>> GetRecentAlbumsAsync(int count)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .OrderByDescending(a => a.ReleaseYear)
                .ThenByDescending(a => a.AlbumId)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Album> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            var queryable = _context.Albums.Include(a => a.Artist).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(a => a.Title.Contains(search) || (a.Artist != null && a.Artist.Name.Contains(search)));
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderByDescending(a => a.AlbumId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Album album)
        {
            await _context.Albums.AddAsync(album);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Album album)
        {
            _context.Albums.Update(album);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Album album)
        {
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
        }
    }
}
