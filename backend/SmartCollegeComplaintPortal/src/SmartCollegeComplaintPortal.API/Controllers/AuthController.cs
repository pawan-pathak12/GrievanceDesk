using DotNetApiStarterKit.Api.Contracts;
using DotNetApiStarterKit.Business.DTOs.Auth;
using DotNetApiStarterKit.Business.DTOs.Otps;
using DotNetApiStarterKit.Business.Interfaces.Auths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DotNetApiStarterKit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        #region Registration Flow
        [EnableRateLimiting("check-email")]
        [HttpPost("register/check-email")]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailDto dto)
        {
            var result = await _authService.CheckEmailAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [EnableRateLimiting("verify-otp")]
        [HttpPost("register/verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _authService.VerifyOtpAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [EnableRateLimiting("create-password")]
        [HttpPost("register/create-password")]
        public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordDto dto)
        {
            var result = await _authService.CreatePasswordAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        #endregion

        #region Login
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, new List<string> { result.Error! }));

            return Ok(new ApiResponse(true, result.StatusCode, new { accessToken = result.AccessToken }));
        }

        #endregion

        #region Password Management
        [EnableRateLimiting("change-password")]
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var result = await _authService.ChangePasswordAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        #endregion

        #region User Info

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _authService.GetMeAsync();

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        #endregion
    }
}