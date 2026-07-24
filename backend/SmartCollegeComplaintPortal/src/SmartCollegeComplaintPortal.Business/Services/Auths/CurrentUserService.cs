using DotNetApiStarterKit.Business.Interfaces.Auths;
using DotNetApiStarterKit.Domain.Constants;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DotNetApiStarterKit.Business.Services.Auths
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimConstants.UserId)?.Value;

            return int.TryParse(value, out var id) ? id : null;
        }

        public string? GetEmail()
        {
            return _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimConstants.Email)?.Value;
        }

        public string? GetRole()
        {
            return _httpContextAccessor.HttpContext?
                .User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}