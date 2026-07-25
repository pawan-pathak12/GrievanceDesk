using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SmartCollegeComplaintPortal.API.Contracts;
using SmartCollegeComplaintPortal.Business.DTOs.Notices;
using SmartCollegeComplaintPortal.Business.Interfaces.Notices;

namespace SmartCollegeComplaintPortal.API.Controllers
{
    [Route("api/admin/notices")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general-policy")]
    public class AdminNoticesController : ControllerBase
    {
        private readonly INoticeService _noticeService;

        public AdminNoticesController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return ToActionResult(await _noticeService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return ToActionResult(await _noticeService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNoticeDto request)
        {
            return ToActionResult(await _noticeService.CreateAsync(request));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNoticeDto request)
        {
            return ToActionResult(await _noticeService.UpdateAsync(id, request));
        }

        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            return ToActionResult(await _noticeService.PublishAsync(id));
        }

        [HttpPatch("{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            return ToActionResult(await _noticeService.UnpublishAsync(id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return ToActionResult(await _noticeService.SoftDeleteAsync(id));
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
