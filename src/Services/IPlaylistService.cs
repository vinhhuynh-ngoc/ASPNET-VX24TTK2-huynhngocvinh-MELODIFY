using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;

namespace Melodify.Services
{
    public interface IPlaylistService
    {
        Task<IEnumerable<PlaylistDto>> GetUserPlaylistsAsync(string userId);
        Task<PlaylistDto?> GetPlaylistByIdAsync(int id);
        Task<PlaylistDto?> GetPlaylistDetailsAsync(int id, string userId);
        Task<PlaylistDto> CreatePlaylistAsync(string userId, string name);
        Task DeletePlaylistAsync(int id);
        Task AddTrackToPlaylistAsync(int playlistId, int trackId);
        Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId);
    }
}
