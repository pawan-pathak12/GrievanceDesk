using AutoMapper;
using DotNetApiStarterKit.Business.DTOs.Users;
using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Business.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserResponseDto>();
        }
    }
}