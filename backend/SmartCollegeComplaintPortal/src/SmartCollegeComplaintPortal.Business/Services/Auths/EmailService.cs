using Microsoft.Extensions.Configuration;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using System.Net;
using System.Net.Mail;

namespace SmartCollegeComplaintPortal.Business.Services.Auths
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // this is base method that is called in every interface method to send mail
        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var host = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]!);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:SenderPassword"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(
                    senderEmail,
                    password),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(
                    senderEmail!,
                    senderName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(to);
            await client.SendMailAsync(message);
        }

        public async Task SendOtpEmailAsync(string to, string otp)
        {
            var subject = "Your OTP - GrievanceDesk";
            var html = $@"
        <div style='font-family:Arial,sans-serif;max-width:500px;
            margin:0 auto;padding:24px;'>
            <h2 style='color:#1A1D2E;'>GrievanceDesk</h2>
            <p>Your OTP code is:</p>
            <div style='font-size:36px;font-weight:bold;
                letter-spacing:8px;color:#5B4FE8;
                padding:16px;background:#F3F4F6;
                border-radius:8px;text-align:center;'>
                {otp}
            </div>
            <p style='color:#9CA3AF;font-size:13px;
                margin-top:16px;'>
                This OTP expires in 10 minutes.<br/>
                If you did not request this, ignore this email.
            </p>
        </div>";

            await SendEmailAsync(to, subject, html);
        }

        public async Task SendTemporaryPasswordAsync(
      string email,
      string fullName,
      string temporaryPassword)
        {
            const string subject =
                "Your Temporary Password - GrievanceDesk";

            var safeFullName = WebUtility.HtmlEncode(fullName);
            var safeEmail = WebUtility.HtmlEncode(email);
            var safePassword = WebUtility.HtmlEncode(temporaryPassword);

            var html = $@"
        <div style='font-family:Arial,sans-serif;
                    max-width:520px;
                    margin:0 auto;
                    padding:24px;
                    color:#1A1D2E;'>

            <h2 style='margin-bottom:24px;'>
                Welcome to GrievanceDesk
            </h2>

            <p>Hello {safeFullName},</p>

            <p>
                An administrator has created an account for you.
            </p>

            <div style='margin-top:24px;
                        padding:18px;
                        background:#F3F4F6;
                        border-radius:8px;'>

                <p style='margin:0 0 16px 0;'>
                    <strong>Email:</strong><br/>
                    {safeEmail}
                </p>

                <p style='margin:0;'>
                    <strong>Temporary Password:</strong>
                </p>

                <div style='font-size:22px;
                            font-weight:bold;
                            letter-spacing:2px;
                            color:#5B4FE8;
                            margin-top:8px;'>
                    {safePassword}
                </div>
            </div>

            <p style='margin-top:24px;'>
                Please log in using the temporary password and change
                the password immediately.
            </p>

            <p style='color:#9CA3AF;
                      font-size:13px;
                      margin-top:20px;'>
                For security, do not share this password with anyone.
                If you were not expecting this account, please contact
                the administrator.
            </p>

            <p style='margin-top:24px;'>
                Thank you.
            </p>
        </div>";

            await SendEmailAsync(email, subject, html);
        }
    }
}