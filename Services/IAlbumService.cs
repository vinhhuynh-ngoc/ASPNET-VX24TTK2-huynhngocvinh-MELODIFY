using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;

namespace Melodify.Services
{
    public interface IAlbumService
    {
        Task<IEnumerable<AlbumDto>> GetAllAlbumsAsync();
        Task<AlbumDto?> GetAlbumByIdAsync(int id);
        Task<AlbumDto?> GetAlbumDetailsAsync(int id, string userId);
        Task<IEnumerable<AlbumDto>> GetRecentAlbumsAsync(int count);
        Task<PagedResult<AlbumDto>> GetPagedAdminAlbumsAsync(int page, int pageSize, string? search);
        Task AddAlbumAsync(Album album);
        Task UpdateAlbumAsync(Album album);
        Task DeleteAlbumAsync(int id);
    }
}
