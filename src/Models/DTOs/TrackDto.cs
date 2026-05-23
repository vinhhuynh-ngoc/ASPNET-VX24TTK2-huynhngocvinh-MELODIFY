namespace Melodify.Models.DTOs
{
    public class TrackDto
    {
        public int TrackId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int? AlbumId { get; set; }
        public string? AlbumTitle { get; set; }
        public string? Genre { get; set; }
        public int Duration { get; set; }
        public string AudioUrl { get; set; }
        public string? CoverImage { get; set; }
        public int PlayCount { get; set; }
        public bool IsLiked { get; set; }
    }
}
