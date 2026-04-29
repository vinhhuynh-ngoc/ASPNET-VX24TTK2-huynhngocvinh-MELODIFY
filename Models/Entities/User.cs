using Microsoft.AspNetCore.Identity;

namespace Melodify.Models.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<LikedTrack> LikedTracks { get; set; }
        public ICollection<FollowArtist> FollowedArtists { get; set; }
    }
}
