using DotNetApiStarterKit.Domain.Entities.Auths;

namespace DotNetApiStarterKit.Business.Interfaces.Auths
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);

        string GenerateVerificationToken(string email);

        bool TryValidateVerificationToken(string token, out string? email);
    }
}