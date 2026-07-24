using DotNetApiStarterKit.Domain.Commons.Pagination;
using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Data.Interfaces
{
    public interface IUserRepository
    {
        #region CRUD Operations

        Task<int> AddAsync(User user);

        Task<PagedResult<User>> GetAllAsync(UserQueryParams query);

        Task<User?> GetByIdAsync(int userId);

        Task<bool> UpdateAsync(int id, User user);

        Task<bool> DeleteAsync(int userId);

        #endregion

        #region User Specific Operations

        Task<string?> GetUserFullNameAsync(int userId);

        Task<bool> CheckEmailExistsAsync(string email);

        Task<User?> GetByEmailAddressAsync(string? email);

        Task<bool> UpdatePasswordAsync(int userId, string newPassword);

        #endregion
    }
}