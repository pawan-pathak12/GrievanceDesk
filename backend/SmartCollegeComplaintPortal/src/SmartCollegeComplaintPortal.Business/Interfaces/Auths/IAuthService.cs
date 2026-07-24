using DotNetApiStarterKit.Business.DTOs.Auth;
using DotNetApiStarterKit.Business.DTOs.Otps;
using DotNetApiStarterKit.Domain.Commons.Pagination;

namespace DotNetApiStarterKit.Business.Interfaces.Auths
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