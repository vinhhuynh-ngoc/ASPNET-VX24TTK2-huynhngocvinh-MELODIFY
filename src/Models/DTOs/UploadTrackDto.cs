using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Melodify.Models.DTOs
{
    public class UploadTrackDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int ArtistId { get; set; }

        public int? AlbumId { get; set; }

        public string? Genre { get; set; }

        [Required]
        public IFormFile AudioFile { get; set; }

        public IFormFile? CoverFile { get; set; }
    }
}
