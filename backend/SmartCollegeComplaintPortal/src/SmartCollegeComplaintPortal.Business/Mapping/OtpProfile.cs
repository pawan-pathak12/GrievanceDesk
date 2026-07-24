using AutoMapper;
using DotNetApiStarterKit.Business.DTOs.Otps;
using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Business.Mapping
{
    public class OtpProfile : Profile
    {
        public OtpProfile()
        {
            CreateMap<Otp, OtpResponseDto>();
            CreateMap<GenerateOtpDto, Otp>();
        }
    }
}