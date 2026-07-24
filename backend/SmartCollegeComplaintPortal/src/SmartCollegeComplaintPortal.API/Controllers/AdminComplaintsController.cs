using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/admin/complaints")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general-policy")]
    public class AdminComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public AdminComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? status, [FromQuery] int? category, [FromQuery] string? search)
        {
            var result = await _complaintService.GetAllAsync(status, category, search);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("irrelevant")]
        public async Task<IActionResult> GetAllIrrelevant()
        {
            var result = await _complaintService.GetAllIrrelevantAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _complaintService.GetComplaintByIdAsync(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplaint(int id, [FromBody] AdminComplaintUpdateRequest request)
        {
            var result = await _complaintService.UpdateAdminComplaintAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }
    }
}
