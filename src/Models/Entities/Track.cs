namespace Melodify.Models.Entities
{
    public class Track
    {
        public int TrackId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public string? Genre { get; set; }
        public int Duration { get; set; }
        public string AudioUrl { get; set; }
        public string? CoverImage { get; set; }
        public int PlayCount { get; set; }
        public Artist Artist { get; set; }
        public Album? Album { get; set; }
    }
}
