using AutoMapper;
using FCxLabs.Domain.Entities;
using FCxLabs.Application.DTOs;

namespace FCxLabs.Application.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile() {
            CreateMap<ApplicationUser, ApplicationUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserFilterDTO>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserUpdateDTO>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserRegisterDTO>().ReverseMap();
        }
    }
}
