using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;

namespace Melodify.Services
{
    public interface IArtistService
    {
        Task<IEnumerable<ArtistDto>> GetAllArtistsAsync(string userId);
        Task<ArtistDto?> GetArtistByIdAsync(int id, string userId);
        Task<ArtistDto?> GetArtistDetailsAsync(int id, string userId);
        Task<IEnumerable<ArtistDto>> GetSuggestedArtistsAsync(int count, string userId);
        Task<PagedResult<ArtistDto>> GetPagedAdminArtistsAsync(int page, int pageSize, string? search);
        Task AddArtistAsync(Artist artist);
        Task UpdateArtistAsync(Artist artist);
        Task DeleteArtistAsync(int id);
        Task FollowArtistAsync(string userId, int artistId);
        Task UnfollowArtistAsync(string userId, int artistId);
    }
}
