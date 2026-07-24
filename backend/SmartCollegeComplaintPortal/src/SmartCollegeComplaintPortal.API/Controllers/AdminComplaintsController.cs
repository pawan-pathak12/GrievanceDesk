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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _complaintService.UpdateStatusAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }

        [HttpPost("{id}/notes")]
        public async Task<IActionResult> AddNote(int id, [FromBody] AddNoteRequest request)
        {
            var result = await _complaintService.AddNoteAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }

        [HttpPut("{id}/duplicate")]
        public async Task<IActionResult> MarkDuplicate(int id, [FromBody] MarkDuplicateRequest request)
        {
            var result = await _complaintService.MarkDuplicateAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }

        [HttpPut("{id}/irrelevant")]
        public async Task<IActionResult> MarkIrrelevant(int id, [FromBody] MarkIrrelevantRequest request)
        {
            var result = await _complaintService.MarkIrrelevantAsync(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }
            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }
    }
}
