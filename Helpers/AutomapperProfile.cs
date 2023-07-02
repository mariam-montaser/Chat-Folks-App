using System.Linq;
using AutoMapper;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Extensions;

namespace SocialApp.Helpers
{
    public class AutomapperProfile: Profile
    {
        public AutomapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, option => option.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain).Url))
                .ForMember(dest => dest.Age, option => option.MapFrom(source => source.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDTO, AppUser>();
        }
    }
}
