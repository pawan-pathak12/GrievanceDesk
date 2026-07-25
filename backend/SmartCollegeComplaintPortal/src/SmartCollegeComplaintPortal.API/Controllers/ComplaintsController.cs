using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    [EnableRateLimiting("general-policy")]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintsController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateComplaintRequest request)
        {
            var result = await _complaintService.CreateAsync(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { id = result.Id, message = result.Message }));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var result = await _complaintService.GetMyComplaintsAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllComplaints()
        {
            var result = await _complaintService.GetAllForUsersAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMyComplaintById(int id)
        {
            var result = await _complaintService.GetMyComplaintByIdAsync(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateComplaintRequest request)
        {
            var result = await _complaintService.UpdateAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _complaintService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }

        [HttpGet("my/irrelevant")]
        public async Task<IActionResult> GetMyIrrelevantComplaints()
        {
            var result = await _complaintService.GetMyIrrelevantAsync();
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }
    }
}
