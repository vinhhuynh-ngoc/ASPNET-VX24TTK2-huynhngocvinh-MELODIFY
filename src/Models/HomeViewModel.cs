using System.Collections.Generic;
using Melodify.Models.DTOs;

namespace Melodify.Models
{
    public class HomeViewModel
    {
        public string Greeting { get; set; }
        public List<AlbumDto> RecentAlbums { get; set; } = new List<AlbumDto>();
        public List<TrackDto> FeaturedTracks { get; set; } = new List<TrackDto>();
        public List<ArtistDto> SuggestedArtists { get; set; } = new List<ArtistDto>();
        public List<PlaylistDto> FeaturedPlaylists { get; set; } = new List<PlaylistDto>();
    }
}
