using SmartCollegeComplaintPortal.Business.DTOs.Otps;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Auths
{
    public interface IOtpService
    {
        Task<Result> GenerateAsync(GenerateOtpDto dto);

        Task<Result> GenerateForPasswordResetAsync(GenerateOtpDto dto);

        Task<Result> VerifyAsync(VerifyOtpDto dto);
    }
}
