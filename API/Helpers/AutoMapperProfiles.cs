using System;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles:Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
        .ForMember(x=>x.Age,o=>o.MapFrom(s=>s.DateOfBirth.CalculateAge()))
        .ForMember(x=>x.PhotoUrl,o=>o.MapFrom(s=>s.Photos.FirstOrDefault(y=>y.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
    }

}
