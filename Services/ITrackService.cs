using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;

namespace Melodify.Services
{
    public interface ITrackService
    {
        Task<IEnumerable<TrackDto>> GetAllTracksAsync(string userId);
        Task<TrackDto?> GetTrackByIdAsync(int id, string userId);
        Task<IEnumerable<TrackDto>> GetFeaturedTracksAsync(int count, string userId);
        Task<PagedResult<TrackDto>> SearchTracksAsync(string query, string userId, int page = 1, int pageSize = 20);
        Task<PagedResult<TrackDto>> GetPagedAdminTracksAsync(int page, int pageSize, string? search, int? artistId, string? sortBy);
        Task IncrementPlayCountAsync(int id);
        Task AddTrackAsync(Track track);
        Task UpdateTrackAsync(Track track);
        Task DeleteTrackAsync(int id);
    }
}
