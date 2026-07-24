using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general-policy")]
    public class DashboardController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public DashboardController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserDashboard()
        {
            var result = await _complaintService.GetUserDashboardAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _complaintService.GetAdminDashboardAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }
    }
}
