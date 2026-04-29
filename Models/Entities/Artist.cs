namespace Melodify.Models.Entities
{
    public class Artist
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public int MonthlyListeners { get; set; }
        public string? Bio { get; set; }
        public ICollection<Track> Tracks { get; set; }
        public ICollection<Album> Albums { get; set; }
    }
}
