using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public interface IArtistRepository
    {
        Task<IEnumerable<Artist>> GetAllAsync();
        Task<Artist?> GetByIdAsync(int id);
        Task<Artist?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Artist>> GetSuggestedArtistsAsync(int count);
        Task<(IEnumerable<Artist> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null);
        Task AddAsync(Artist artist);
        Task UpdateAsync(Artist artist);
        Task DeleteAsync(Artist artist);
    }
}
