using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/complaints")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general-policy")]
    public class ComplaintCommentsController : ControllerBase
    {
        private readonly IComplaintCommentService _commentService;

        public ComplaintCommentsController(IComplaintCommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("{complaintId}/comments")]
        public async Task<IActionResult> GetByComplaintId(int complaintId)
        {
            var result = await _commentService.GetByComplaintIdAsync(complaintId);
            return ToActionResult(result);
        }

        [HttpPost("{complaintId}/comments")]
        public async Task<IActionResult> Create(int complaintId, [FromBody] CreateComplaintCommentRequest request)
        {
            var result = await _commentService.CreateAsync(complaintId, request);
            return ToActionResult(result);
        }

        [HttpPut("comments/{commentId}")]
        public async Task<IActionResult> Update(int commentId, [FromBody] UpdateComplaintCommentRequest request)
        {
            var result = await _commentService.UpdateAsync(commentId, request);
            return ToActionResult(result);
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteOwn(int commentId)
        {
            var result = await _commentService.DeleteOwnAsync(commentId);
            return ToActionResult(result);
        }

        private IActionResult ToActionResult(SmartCollegeComplaintPortal.Domain.Commons.Pagination.Result result)
        {
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }

            return Ok(new ApiResponse(true, result.StatusCode, result.Data ?? new { message = result.Message }));
        }
    }
}
