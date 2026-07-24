using SmartCollegeComplaintPortal.Business.DTOs.Users;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Auths
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