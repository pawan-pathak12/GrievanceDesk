using AutoMapper;
using SmartCollegeComplaintPortal.Business.DTOs.Otps;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Business.Mapping
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