using DotNetApiStarterKit.Business.DTOs.Otps;
using DotNetApiStarterKit.Domain.Commons.Pagination;

namespace DotNetApiStarterKit.Business.Interfaces.Auths
{
    public interface IOtpService
    {
        Task<Result> GenerateAsync(GenerateOtpDto dto);

        Task<Result> VerifyAsync(VerifyOtpDto dto);
    }
}