using SmartCollegeComplaintPortal.Domain.Entities.Auths;

namespace SmartCollegeComplaintPortal.Business.Interfaces.Auths
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);

        string GenerateVerificationToken(string email);

        bool TryValidateVerificationToken(string token, out string? email);

        string GeneratePasswordResetToken(string email);

        bool TryValidatePasswordResetToken(string token, out string? email);
    }
}
