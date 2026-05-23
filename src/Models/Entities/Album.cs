namespace Melodify.Models.Entities
{
    public class Album
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public string? CoverImage { get; set; }
        public int ReleaseYear { get; set; }
        public Artist Artist { get; set; }
        public ICollection<Track> Tracks { get; set; }
    }
}
