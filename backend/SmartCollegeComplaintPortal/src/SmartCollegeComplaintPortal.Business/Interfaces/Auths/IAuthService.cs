using SmartCollegeComplaintPortal.Business.DTOs.Auth;
using SmartCollegeComplaintPortal.Business.DTOs.Otps;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Auths
{
    public interface IAuthService
    {
        Task<Result> CheckEmailAsync(CheckEmailDto dto);

        Task<Result> VerifyOtpAsync(VerifyOtpDto dto);

        Task<Result> CreatePasswordAsync(CreatePasswordDto dto);

        Task<AuthResult> LoginAsync(LoginDto dto);

        Task<Result> ChangePasswordAsync(ChangePasswordDto dto);

        Task<Result> GetMeAsync();
    }
}