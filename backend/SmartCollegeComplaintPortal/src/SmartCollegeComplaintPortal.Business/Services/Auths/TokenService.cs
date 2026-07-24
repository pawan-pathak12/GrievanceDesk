using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Domain.Constants;
using SmartCollegeComplaintPortal.Domain.Entities.Auths;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartCollegeComplaintPortal.Business.Services.Auths
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private const string VerificationPurpose = "otp_verified";
        private const string PasswordResetPurpose = "password_reset";
        private const int VerificationTokenExpiryMinutes = 10;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            this._logger = logger;
        }

        #region Access Token

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");


            //1.claims 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub ,user.UserId.ToString()) ,
                new Claim(JwtRegisteredClaimNames.Email ,user.Email) ,
                new Claim("username", user.Username ?? string.Empty),
                new Claim (ClaimTypes.Role , user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString())
            };
            //2. prove that : their is use of righr jwt secret key to generate token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["key"]!)
                );

            //3. Credentials 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(jwtSettings["ExpiresInMinutes"]!)
                    ),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
        #endregion

        #region Verification Token

        public string GenerateVerificationToken(string email)
        {
            return GeneratePurposeToken(email, VerificationPurpose);
        }

        public string GeneratePasswordResetToken(string email)
        {
            return GeneratePurposeToken(email, PasswordResetPurpose);
        }

        private string GeneratePurposeToken(string email, string purpose)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimConstants.Email, email),
                new(ClaimConstants.Purpose, purpose)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(VerificationTokenExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool TryValidateVerificationToken(string token, out string? email)
        {
            return TryValidatePurposeToken(token, VerificationPurpose, out email);
        }

        public bool TryValidatePasswordResetToken(string token, out string? email)
        {
            return TryValidatePurposeToken(token, PasswordResetPurpose, out email);
        }

        private bool TryValidatePurposeToken(string token, string expectedPurpose, out string? email)
        {
            email = null;

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Verification token validation failed. Token is empty.");
                return false;
            }

            token = token.Trim();

            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("Verification token validation failed. Jwt key is empty.");
                return false;
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var handler = new JwtSecurityTokenHandler
                {
                    MapInboundClaims = false
                };

                var principal = handler.ValidateToken(
                    token,
                    validationParameters,
                    out _);

                var purpose = principal.FindFirst(ClaimConstants.Purpose)?.Value;

                if (purpose != expectedPurpose)
                {
                    _logger.LogWarning(
                        "Verification token validation failed. Invalid purpose. Expected: {ExpectedPurpose}, Actual: {ActualPurpose}",
                        expectedPurpose,
                        purpose);

                    return false;
                }

                var emailClaim = principal.FindFirst(ClaimConstants.Email)?.Value;

                if (string.IsNullOrWhiteSpace(emailClaim))
                {
                    _logger.LogWarning("Verification token validation failed. Email claim missing.");
                    return false;
                }

                email = emailClaim;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Verification token validation failed. Invalid or expired token.");

                return false;
            }
        }


        #endregion
    }
}
