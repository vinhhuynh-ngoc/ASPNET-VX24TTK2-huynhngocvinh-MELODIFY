using System.Collections.Generic;

namespace Melodify.Models.DTOs
{
    public class AlbumDto
    {
        public int AlbumId { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImage { get; set; }
        public int ReleaseYear { get; set; }
        public List<TrackDto> Tracks { get; set; } = new List<TrackDto>();
    }
}
