namespace Melodify.Models.Entities
{
    public class LikedTrack
    {
        public string UserId { get; set; }
        public int TrackId { get; set; }
        public DateTime LikedAt { get; set; }
        public User User { get; set; }
        public Track Track { get; set; }
    }
}
