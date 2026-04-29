using System;
using System.Collections.Generic;

namespace Melodify.Models.DTOs
{
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TrackDto> Tracks { get; set; } = new List<TrackDto>();
    }
}
