using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public interface IAlbumRepository
    {
        Task<IEnumerable<Album>> GetAllAsync();
        Task<Album?> GetByIdAsync(int id);
        Task<Album?> GetByIdWithTracksAsync(int id);
        Task<IEnumerable<Album>> GetFeaturedAlbumsAsync(int count);
        Task<IEnumerable<Album>> GetRecentAlbumsAsync(int count);
        Task<(IEnumerable<Album> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null);
        Task AddAsync(Album album);
        Task UpdateAsync(Album album);
        Task DeleteAsync(Album album);
    }
}
