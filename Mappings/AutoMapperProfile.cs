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
                .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist != null ? src.Artist.Name : string.Empty))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.Tracks));

            CreateMap<Playlist, PlaylistDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.Tracks, opt => opt.MapFrom(src => src.PlaylistTracks.Select(pt => pt.Track)));
        }
    }
}
