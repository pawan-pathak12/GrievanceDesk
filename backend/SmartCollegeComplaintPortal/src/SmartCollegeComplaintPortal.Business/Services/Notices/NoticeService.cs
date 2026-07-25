using AutoMapper;
using SmartCollegeComplaintPortal.Business.DTOs.Notices;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Business.Interfaces.Notices;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Notices;

namespace SmartCollegeComplaintPortal.Business.Services.Notices
{
    public class NoticeService : INoticeService
    {
        private readonly INoticeRepository _noticeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public NoticeService(
            INoticeRepository noticeRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _noticeRepository = noticeRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Result> CreateAsync(CreateNoticeDto request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var now = DateTime.UtcNow;
            var notice = _mapper.Map<Notice>(request);
            notice.Title = request.Title.Trim();
            notice.Content = request.Content.Trim();
            notice.CreatedBy = userId.Value;
            notice.CreatedAt = now;
            notice.IsPublished = false;
            notice.PublishedAt = null;

            await _noticeRepository.CreateAsync(notice);
            return Result.Success(_mapper.Map<NoticeResponseDto>(notice), "Notice created successfully.");
        }

        public async Task<Result> UpdateAsync(int noticeId, UpdateNoticeDto request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var notice = await _noticeRepository.GetByIdAsync(noticeId);
            if (notice == null)
            {
                return Result.NotFound($"Notice with ID {noticeId} was not found.");
            }

            _mapper.Map(request, notice);
            notice.Title = request.Title.Trim();
            notice.Content = request.Content.Trim();
            notice.UpdatedBy = userId.Value;
            notice.UpdatedAt = DateTime.UtcNow;

            await _noticeRepository.UpdateAsync(notice);
            return Result.Success(_mapper.Map<NoticeResponseDto>(notice), "Notice updated successfully.");
        }

        public Task<Result> PublishAsync(int noticeId) => SetPublicationAsync(noticeId, true);

        public Task<Result> UnpublishAsync(int noticeId) => SetPublicationAsync(noticeId, false);

        public async Task<Result> SoftDeleteAsync(int noticeId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var deleted = await _noticeRepository.SoftDeleteAsync(noticeId, userId.Value, DateTime.UtcNow);
            return deleted
                ? Result.Success("Notice archived successfully.")
                : Result.NotFound($"Notice with ID {noticeId} was not found.");
        }

        public async Task<Result> GetAllAsync()
        {
            var notices = await _noticeRepository.GetAllAsync();
            return Result.Success(_mapper.Map<List<NoticeResponseDto>>(notices));
        }

        public async Task<Result> GetByIdAsync(int noticeId)
        {
            var notice = await _noticeRepository.GetByIdAsync(noticeId);
            return notice == null
                ? Result.NotFound($"Notice with ID {noticeId} was not found.")
                : Result.Success(_mapper.Map<NoticeResponseDto>(notice));
        }

        public async Task<Result> GetPublishedAsync()
        {
            var notices = await _noticeRepository.GetPublishedAsync();
            return Result.Success(_mapper.Map<List<UserNoticeDto>>(notices));
        }

        public async Task<Result> GetPublishedByIdAsync(int noticeId)
        {
            var notice = await _noticeRepository.GetByIdAsync(noticeId);
            if (notice == null || !notice.IsPublished)
            {
                return Result.NotFound($"Published notice with ID {noticeId} was not found.");
            }

            return Result.Success(_mapper.Map<UserNoticeDetailDto>(notice));
        }

        private async Task<Result> SetPublicationAsync(int noticeId, bool publish)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var notice = await _noticeRepository.GetByIdAsync(noticeId);
            if (notice == null)
            {
                return Result.NotFound($"Notice with ID {noticeId} was not found.");
            }

            if (notice.IsPublished == publish)
            {
                return Result.Success(_mapper.Map<NoticeResponseDto>(notice), publish ? "Notice is already published." : "Notice is already a draft.");
            }

            notice.IsPublished = publish;
            notice.PublishedAt = publish ? DateTime.UtcNow : null;
            notice.UpdatedAt = DateTime.UtcNow;
            notice.UpdatedBy = userId.Value;

            await _noticeRepository.UpdateAsync(notice);
            return Result.Success(_mapper.Map<NoticeResponseDto>(notice), publish ? "Notice published successfully." : "Notice unpublished successfully.");
        }
    }
}
