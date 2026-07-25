using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/admin/comments")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general-policy")]
    public class AdminCommentsController : ControllerBase
    {
        private readonly IComplaintCommentService _commentService;

        public AdminCommentsController(IComplaintCommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> Delete(int commentId)
        {
            var result = await _commentService.DeleteAsAdminAsync(commentId);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }

            return Ok(new ApiResponse(true, result.StatusCode, new { message = result.Message }));
        }
    }
}
