namespace Melodify.Models.Entities
{
    public class FollowArtist
    {
        public string UserId { get; set; } = null!;
        public int ArtistId { get; set; }
        public DateTime FollowedAt { get; set; }
        public User? User { get; set; }
        public Artist? Artist { get; set; }
    }
}
