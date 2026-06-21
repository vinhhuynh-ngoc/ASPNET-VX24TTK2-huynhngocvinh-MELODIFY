using AutoMapper;
using System.Linq;
using Melodify.Models.Entities;
using Melodify.Models.DTOs;

namespace Melodify.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Track, TrackDto>()
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist != null ? src.Artist.Name : string.Empty))
                .ForMember(dest => dest.AlbumTitle, opt => opt.MapFrom(src => src.Album != null ? src.Album.Title : string.Empty))
                .ForMember(dest => dest.IsLiked, opt => opt.Ignore());

            CreateMap<Artist, ArtistDto>()
                .ForMember(dest => dest.IsFollowed, opt => opt.Ignore());

            CreateMap<Album, AlbumDto>()
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(src =>
                    src.Tracks != null && src.Tracks.Any(t => t.Artist != null)
                        ? src.Tracks.Select(t => t.Artist).Where(a => a != null).GroupBy(a => a.ArtistId).Select(g => g.First()).ToList()
                        : (src.Artist != null ? new List<Artist> { src.Artist } : new List<Artist>())))
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src =>
                    src.Tracks != null && src.Tracks.Any(t => t.Artist != null)
                        ? string.Join(", ", src.Tracks.Select(t => t.Artist.Name).Distinct())
                        : (src.Artist != null ? src.Artist.Name : string.Empty)))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));

            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.PlaylistTracks.Select(pt => pt.Track)));
        }
    }
}
