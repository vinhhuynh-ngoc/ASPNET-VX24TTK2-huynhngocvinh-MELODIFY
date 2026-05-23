using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Melodify.Models.DTOs
{
    public class EditTrackDto
    {
        public int TrackId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public int? AlbumId { get; set; }

        public string? Genre { get; set; }

        public IFormFile? NewAudioFile { get; set; }

        public IFormFile? NewCoverFile { get; set; }
    }
}
