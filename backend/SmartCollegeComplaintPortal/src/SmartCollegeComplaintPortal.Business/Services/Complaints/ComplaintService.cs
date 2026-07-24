using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Business.Helpers;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Business.Services.Complaints
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly ILogger<ComplaintService> _logger;

        public ComplaintService(
            IComplaintRepository complaintRepository,
            ICurrentUserService currentUserService,
            IMapper mapper,
            ILogger<ComplaintService> logger)
        {
            _complaintRepository = complaintRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _logger = logger;
        }

        #region User Operations

        public async Task<Result> CreateAsync(CreateComplaintRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            // Validate content
            var contentCheck = ContentValidator.ValidateComplaint(request.Title, request.Description);
            if (!contentCheck.IsClean)
            {
                _logger.LogWarning(
                    "Complaint creation blocked. Content validation failed. UserId: {UserId}",
                    userId.Value);

                return Result.Failure(contentCheck.Message!);
            }

            var complaint = new Complaint
            {
                UserId = userId.Value,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Category = request.Category,
                Location = request.Location.Trim(),
                Status = ComplaintStatus.Pending,
                IsDuplicate = false,
                IsEdited = false,
                CreatedAt = DateTime.UtcNow
            };

            var complaintId = await _complaintRepository.CreateAsync(complaint);

            if (complaintId <= 0)
            {
                _logger.LogWarning(
                    "Complaint creation failed in repository. UserId: {UserId}",
                    userId.Value);

                return Result.Failure("Complaint creation failed.");
            }

            // Create audit trail entry
            var update = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.Created,
                NewStatus = ComplaintStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _complaintRepository.CreateUpdateAsync(update);

            _logger.LogInformation(
                "Complaint created successfully. ComplaintId: {ComplaintId}, UserId: {UserId}",
                complaintId, userId.Value);

            var result = Result.Success("Complaint submitted successfully.");
            result.Id = complaintId;
            return result;
        }

        public async Task<Result> GetMyComplaintsAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var complaints = await _complaintRepository.GetByUserIdAsync(userId.Value);
            var response = _mapper.Map<List<ComplaintSummaryResponse>>(complaints);

            return Result.Success(response);
        }

        public async Task<Result> GetMyComplaintByIdAsync(int complaintId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var complaint = await _complaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            // Verify ownership
            if (complaint.UserId != userId.Value)
            {
                _logger.LogWarning(
                    "Unauthorized access attempt. UserId: {UserId}, ComplaintId: {ComplaintId}",
                    userId.Value, complaintId);

                return Result.Forbidden("You do not have access to this complaint.");
            }

            var response = _mapper.Map<ComplaintDetailResponse>(complaint);

            // Include updates (history timeline)
            var updates = await _complaintRepository.GetUpdatesByComplaintIdAsync(complaintId);
            response.Updates = _mapper.Map<List<ComplaintUpdateResponse>>(updates);

            return Result.Success(response);
        }

        public async Task<Result> UpdateAsync(int complaintId, UpdateComplaintRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            // Verify ownership
            var ownerUserId = await _complaintRepository.GetOwnerUserIdAsync(complaintId);
            if (ownerUserId == null)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            if (ownerUserId.Value != userId.Value)
            {
                return Result.Forbidden("You do not have access to this complaint.");
            }

            // Check status == Pending
            var status = await _complaintRepository.GetStatusAsync(complaintId);
            if (status == null || status.Value != (int)ComplaintStatus.Pending)
            {
                return Result.Failure("Only complaints with Pending status can be edited.");
            }

            // Validate content
            var contentCheck = ContentValidator.ValidateComplaint(request.Title, request.Description);
            if (!contentCheck.IsClean)
            {
                _logger.LogWarning(
                    "Complaint update blocked. Content validation failed. ComplaintId: {ComplaintId}",
                    complaintId);

                return Result.Failure(contentCheck.Message!);
            }

            var now = DateTime.UtcNow;

            var updated = await _complaintRepository.UpdateAsync(
                complaintId,
                request.Title.Trim(),
                request.Description.Trim(),
                (int)request.Category,
                request.Location.Trim(),
                now);

            if (!updated)
            {
                _logger.LogWarning(
                    "Complaint update failed in repository. ComplaintId: {ComplaintId}",
                    complaintId);

                return Result.Failure("Complaint update failed.");
            }

            // Mark as edited
            await _complaintRepository.MarkAsEditedAsync(complaintId, now);

            // Create audit trail entry
            var updateEntry = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.Edited,
                CreatedAt = now
            };

            await _complaintRepository.CreateUpdateAsync(updateEntry);

            _logger.LogInformation(
                "Complaint updated successfully. ComplaintId: {ComplaintId}, UserId: {UserId}",
                complaintId, userId.Value);

            return Result.Success("Complaint updated successfully.");
        }

        public async Task<Result> DeleteAsync(int complaintId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            // Verify ownership
            var ownerUserId = await _complaintRepository.GetOwnerUserIdAsync(complaintId);
            if (ownerUserId == null)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            if (ownerUserId.Value != userId.Value)
            {
                return Result.Forbidden("You do not have access to this complaint.");
            }

            // Check status == Pending
            var status = await _complaintRepository.GetStatusAsync(complaintId);
            if (status == null || status.Value != (int)ComplaintStatus.Pending)
            {
                return Result.Failure("Only complaints with Pending status can be deleted.");
            }

            var deleted = await _complaintRepository.DeleteAsync(complaintId);
            if (!deleted)
            {
                _logger.LogWarning(
                    "Complaint deletion failed. ComplaintId: {ComplaintId}",
                    complaintId);

                return Result.Failure("Complaint deletion failed.");
            }

            _logger.LogInformation(
                "Complaint deleted successfully. ComplaintId: {ComplaintId}, UserId: {UserId}",
                complaintId, userId.Value);

            return Result.Success("Complaint deleted successfully.");
        }

        public async Task<Result> GetMyIrrelevantAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var complaints = await _complaintRepository.GetIrrelevantByUserIdAsync(userId.Value);
            var response = _mapper.Map<List<ComplaintSummaryResponse>>(complaints);

            return Result.Success(response);
        }

        #endregion

        #region Admin Operations

        public async Task<Result> GetAllAsync(int? status, int? category, string? search)
        {
            var complaints = await _complaintRepository.GetAllAsync(status, category, search);
            var response = _mapper.Map<List<ComplaintSummaryResponse>>(complaints);

            return Result.Success(response);
        }

        public async Task<Result> GetComplaintByIdAsync(int complaintId)
        {
            var complaint = await _complaintRepository.GetByIdAsync(complaintId);
            if (complaint == null)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            var response = _mapper.Map<ComplaintDetailResponse>(complaint);

            // Include updates (history timeline)
            var updates = await _complaintRepository.GetUpdatesByComplaintIdAsync(complaintId);
            response.Updates = _mapper.Map<List<ComplaintUpdateResponse>>(updates);

            return Result.Success(response);
        }

        public async Task<Result> UpdateStatusAsync(int complaintId, UpdateStatusRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var currentStatus = await _complaintRepository.GetStatusAsync(complaintId);
            if (currentStatus == null)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            var now = DateTime.UtcNow;

            var updated = await _complaintRepository.UpdateStatusAsync(
                complaintId, (int)request.NewStatus, now);

            if (!updated)
            {
                return Result.Failure("Status update failed.");
            }

            // Create audit trail entry
            var updateEntry = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.StatusChanged,
                OldStatus = (ComplaintStatus)currentStatus.Value,
                NewStatus = request.NewStatus,
                CreatedAt = now
            };

            await _complaintRepository.CreateUpdateAsync(updateEntry);

            _logger.LogInformation(
                "Complaint status updated. ComplaintId: {ComplaintId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                complaintId, currentStatus.Value, request.NewStatus);

            return Result.Success("Status updated successfully.");
        }

        public async Task<Result> AddNoteAsync(int complaintId, AddNoteRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var exists = await _complaintRepository.ExistsAsync(complaintId);
            if (!exists)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            var updateEntry = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.NoteAdded,
                NoteText = request.NoteText.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _complaintRepository.CreateUpdateAsync(updateEntry);

            _logger.LogInformation(
                "Note added to complaint. ComplaintId: {ComplaintId}",
                complaintId);

            return Result.Success("Note added successfully.");
        }

        public async Task<Result> MarkDuplicateAsync(int complaintId, MarkDuplicateRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var exists = await _complaintRepository.ExistsAsync(complaintId);
            if (!exists)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            var now = DateTime.UtcNow;

            await _complaintRepository.UpdateDuplicateFlagAsync(complaintId, request.IsDuplicate, now);

            var updateEntry = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.MarkedDuplicate,
                NoteText = request.NoteText,
                CreatedAt = now
            };

            await _complaintRepository.CreateUpdateAsync(updateEntry);

            _logger.LogInformation(
                "Complaint marked as duplicate. ComplaintId: {ComplaintId}, IsDuplicate: {IsDuplicate}",
                complaintId, request.IsDuplicate);

            return Result.Success("Complaint duplicate flag updated.");
        }

        public async Task<Result> MarkIrrelevantAsync(int complaintId, MarkIrrelevantRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var exists = await _complaintRepository.ExistsAsync(complaintId);
            if (!exists)
            {
                return Result.NotFound($"Complaint with ID {complaintId} not found.");
            }

            var now = DateTime.UtcNow;

            await _complaintRepository.UpdateStatusAsync(complaintId, (int)ComplaintStatus.Irrelevant, now);

            var updateEntry = new ComplaintUpdate
            {
                ComplaintId = complaintId,
                UpdatedByUserId = userId.Value,
                UpdateType = ComplaintUpdateType.MarkedIrrelevant,
                NoteText = request.Reason,
                NewStatus = ComplaintStatus.Irrelevant,
                CreatedAt = now
            };

            await _complaintRepository.CreateUpdateAsync(updateEntry);

            _logger.LogInformation(
                "Complaint marked as irrelevant. ComplaintId: {ComplaintId}",
                complaintId);

            return Result.Success("Complaint marked as irrelevant.");
        }

        public async Task<Result> GetAllIrrelevantAsync()
        {
            var complaints = await _complaintRepository.GetIrrelevantAsync();
            var response = _mapper.Map<List<ComplaintSummaryResponse>>(complaints);

            return Result.Success(response);
        }

        #endregion

        #region Dashboard

        public async Task<Result> GetUserDashboardAsync()
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null)
            {
                return Result.Unauthorized("User could not be identified.");
            }

            var stats = await _complaintRepository.GetUserDashboardStatsAsync(userId.Value);

            return Result.Success(stats);
        }

        public async Task<Result> GetAdminDashboardAsync()
        {
            var stats = await _complaintRepository.GetAdminDashboardStatsAsync();

            return Result.Success(stats);
        }

        #endregion
    }
}
