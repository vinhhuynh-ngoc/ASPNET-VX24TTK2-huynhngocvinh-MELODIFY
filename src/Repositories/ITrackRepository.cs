using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public interface ITrackRepository
    {
        Task<IEnumerable<Track>> GetAllAsync();
        Task<Track?> GetByIdAsync(int id);
        Task<IEnumerable<Track>> GetFeaturedTracksAsync(int count);
        Task<(IEnumerable<Track> Tracks, int TotalCount)> SearchAsync(string query, int page = 1, int pageSize = 20);
        Task<(IEnumerable<Track> Items, int TotalCount)> GetPagedAdminAsync(int page = 1, int pageSize = 10, string? search = null, int? artistId = null, string? sortBy = null);
        Task AddAsync(Track track);
        Task UpdateAsync(Track track);
        Task DeleteAsync(Track track);
    }
}
