using DotNetApiStarterKit.Domain.Commons.Pagination;
using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Data.Interfaces
{
    public interface IOtpRepository
    {
        #region CRUD Operations

        Task<int> AddAsync(Otp otp);

        Task<PagedResult<Otp>> GetAllAsync(OtpQueryParams query);

        Task<Otp?> GetByIdAsync(int otpId);

        Task<bool> DeleteAsync(int otpId);

        #endregion

        #region Otp Specific Operations

        Task<Otp?> GetByEmailAsync(string email);

        Task<Otp?> GetLatestValidOtpAsync(string email, string code);

        Task<bool> MarkAsUsedAsync(int otpId);

        Task<bool> InvalidatePreviousOtpsAsync(string email);
        Task<int> CountOtpRequestsAsync(string email, DateTime since);

        #endregion
    }
}