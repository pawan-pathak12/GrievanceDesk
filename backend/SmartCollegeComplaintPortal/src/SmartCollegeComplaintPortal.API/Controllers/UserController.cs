using DotNetApiStarterKit.Api.Contracts;
using DotNetApiStarterKit.Business.DTOs.Users;
using DotNetApiStarterKit.Business.Interfaces.Auths;
using DotNetApiStarterKit.Domain.Commons.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DotNetApiStarterKit.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general-policy")]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("/admin/create-new")]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateUserByAdminRequest dto)
        {
            var result = await _userService.CreateUserByAdminAsync(dto);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode,
                    null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParams query)
        {
            var result = await _userService.GetAllAsync(query);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var result = await _userService.GetByEmailAsync(email);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var ownUserIdClaim = User.FindFirst("sub")?.Value;

            if (!int.TryParse(ownUserIdClaim, out var ownUserId))
                return StatusCode(401, new ErrorResponse(false, 401, null, new List<string> { "Invalid user id in token" }));

            if (ownUserId == id)
                return StatusCode(400, new ErrorResponse(false, 400, null, new List<string> { "You cannot delete your own account" }));

            var result = await _userService.DeleteAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }
    }
}