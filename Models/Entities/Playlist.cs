namespace Melodify.Models.Entities
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public ICollection<PlaylistTrack> PlaylistTracks { get; set; }
    }
}
