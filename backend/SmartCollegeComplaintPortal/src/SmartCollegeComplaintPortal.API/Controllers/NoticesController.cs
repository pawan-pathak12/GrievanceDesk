using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.Interfaces.Notices;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/notices")]
    [ApiController]
    [Authorize(Roles = "User,Admin")]
    [EnableRateLimiting("general-policy")]
    public class NoticesController : ControllerBase
    {
        private readonly INoticeService _noticeService;

        public NoticesController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublished()
        {
            return ToActionResult(await _noticeService.GetPublishedAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPublishedById(int id)
        {
            return ToActionResult(await _noticeService.GetPublishedByIdAsync(id));
        }

        private IActionResult ToActionResult(SmartCollegeComplaintPortal.Domain.Commons.Pagination.Result result)
        {
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new ErrorResponse(false, result.StatusCode, null, result.Errors));
            }

            return Ok(new ApiResponse(true, result.StatusCode, result.Data));
        }
    }
}
