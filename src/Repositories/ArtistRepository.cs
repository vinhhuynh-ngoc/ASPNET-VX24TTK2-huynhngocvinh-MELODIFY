using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Melodify.Data;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly AppDbContext _context;

        public ArtistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _context.Artists.ToListAsync();
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _context.Artists.FindAsync(id);
        }

        public async Task<Artist?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Artists
                .Include(a => a.Tracks)
                .Include(a => a.Albums)
                .FirstOrDefaultAsync(a => a.ArtistId == id);
        }

        public async Task<IEnumerable<Artist>> GetSuggestedArtistsAsync(int count)
        {
            return await _context.Artists
                .OrderByDescending(a => a.ArtistId) // Basic sorting, could be randomized or based on popularity
                .Take(count)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Artist> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            var queryable = _context.Artists.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                queryable = queryable.Where(a => a.Name.Contains(search) || (a.Bio != null && a.Bio.Contains(search)));
            }

            var totalCount = await queryable.CountAsync();
            var items = await queryable
                .OrderByDescending(a => a.ArtistId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AddAsync(Artist artist)
        {
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Artist artist)
        {
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Artist artist)
        {
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
        }
    }
}
