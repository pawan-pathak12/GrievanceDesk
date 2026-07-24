using DotNetApiStarterKit.Business.DTOs.Users;
using DotNetApiStarterKit.Domain.Commons.Pagination;

namespace DotNetApiStarterKit.Business.Interfaces.Auths
{
    public interface IUserService
    {
        Task<Result> CreateAsync(CreateUserDto dto);

        Task<Result> GetByIdAsync(int userId);

        Task<Result> GetByEmailAsync(string email);

        Task<Result> GetAllAsync(UserQueryParams query);

        Task<Result> UpdateAsync(int userId, UpdateUserDto dto);

        Task<Result> DeleteAsync(int userId);

        Task<Result> CheckEmailExistsAsync(string email);
        Task<Result> CreateUserByAdminAsync(CreateUserByAdminRequest request);
    }
}