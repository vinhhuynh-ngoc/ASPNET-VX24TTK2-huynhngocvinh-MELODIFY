namespace Melodify.Models.DTOs
{
    public class ArtistDto
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public int MonthlyListeners { get; set; }
        public string? Bio { get; set; }
        public bool IsFollowed { get; set; }
    }
}
