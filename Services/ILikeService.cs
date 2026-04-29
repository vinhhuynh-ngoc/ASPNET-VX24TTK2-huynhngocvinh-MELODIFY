using System.Collections.Generic;
using System.Threading.Tasks;
using Melodify.Models.DTOs;

namespace Melodify.Services
{
    public interface ILikeService
    {
        Task LikeTrackAsync(string userId, int trackId);
        Task UnlikeTrackAsync(string userId, int trackId);
        Task<IEnumerable<TrackDto>> GetLikedTracksAsync(string userId);
    }
}
