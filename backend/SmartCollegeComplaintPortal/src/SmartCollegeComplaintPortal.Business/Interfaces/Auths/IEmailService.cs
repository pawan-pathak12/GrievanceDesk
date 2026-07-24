namespace SmartCollegeComplaintPortal.Business.Interfaces.Auths
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otpCode);
        Task SendTemporaryPasswordAsync(
    string email,
    string fullName,
    string temporaryPassword);
    }
}