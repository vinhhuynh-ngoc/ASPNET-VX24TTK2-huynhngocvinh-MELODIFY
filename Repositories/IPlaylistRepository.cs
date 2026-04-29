using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.Entities;

namespace Melodify.Repositories
{
    public interface IPlaylistRepository
    {
        Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(string userId);
        Task<Playlist?> GetByIdAsync(int id);
        Task<Playlist?> GetByIdWithTracksAsync(int id);
        Task AddAsync(Playlist playlist);
        Task UpdateAsync(Playlist playlist);
        Task DeleteAsync(Playlist playlist);
        Task AddTrackToPlaylistAsync(PlaylistTrack playlistTrack);
        Task RemoveTrackFromPlaylistAsync(int playlistId, int trackId);
        Task<bool> IsTrackInPlaylistAsync(int playlistId, int trackId);
    }
}
