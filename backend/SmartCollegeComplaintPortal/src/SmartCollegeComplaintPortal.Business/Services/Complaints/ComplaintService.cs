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
                Status = ComplaintStatus.Pending,
                IsDuplicate = false,
                IsEdited = false,
                IsActive = true,
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

        public async Task<Result> GetAllForUsersAsync()
        {
            var complaints = await _complaintRepository.GetAllAsync(null, null, null);
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

        public async Task<Result> UpdateAdminComplaintAsync(int complaintId, AdminComplaintUpdateRequest request)
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

            var currentStatus = (ComplaintStatus)complaint.Status;
            if (IsLockedComplaint(currentStatus, complaint.IsDuplicate))
            {
                return Result.Failure("This complaint has been finalized and can no longer be modified.");
            }

            var targetStatus = request.Status;
            if (targetStatus == ComplaintStatus.Irrelevant)
            {
                return Result.Failure("Use the separate irrelevant action to finalize a complaint as irrelevant.");
            }

            if (!IsAllowedStatusTransition(currentStatus, targetStatus))
            {
                return Result.Failure($"A complaint in {currentStatus} status cannot be changed to {targetStatus}.");
            }

            var note = request.Note?.Trim();

            if (targetStatus == ComplaintStatus.Resolved && string.IsNullOrWhiteSpace(note))
            {
                return Result.Failure("A resolution note is required before resolving a complaint.");
            }

            var updates = new List<ComplaintUpdate>();
            var now = DateTime.UtcNow;

            if (targetStatus != currentStatus)
            {
                updates.Add(new ComplaintUpdate
                {
                    ComplaintId = complaintId,
                    UpdatedByUserId = userId.Value,
                    UpdateType = ComplaintUpdateType.StatusChanged,
                    OldStatus = currentStatus,
                    NewStatus = targetStatus,
                    CreatedAt = now
                });
            }

            if (!string.IsNullOrWhiteSpace(note))
            {
                updates.Add(new ComplaintUpdate
                {
                    ComplaintId = complaintId,
                    UpdatedByUserId = userId.Value,
                    UpdateType = ComplaintUpdateType.NoteAdded,
                    NoteText = note,
                    CreatedAt = now
                });
            }

            if (request.IsDuplicate)
            {
                updates.Add(new ComplaintUpdate
                {
                    ComplaintId = complaintId,
                    UpdatedByUserId = userId.Value,
                    UpdateType = ComplaintUpdateType.MarkedDuplicate,
                    CreatedAt = now
                });
            }

            if (updates.Count == 0)
            {
                return Result.Failure("Select a new status, add a note, or mark the complaint before updating.");
            }

            complaint.Status = (int)targetStatus;
            complaint.IsDuplicate = request.IsDuplicate;
            complaint.UpdatedAt = now;

            var updated = await _complaintRepository.ApplyAdminUpdateAsync(
                complaintId,
                complaint.Status,
                complaint.IsDuplicate,
                now,
                updates);
            if (!updated)
            {
                return Result.Failure("Complaint update failed. Please try again.");
            }

            _logger.LogInformation("Admin updated complaint {ComplaintId} with {UpdateCount} actions", complaintId, updates.Count);

            return await GetComplaintByIdAsync(complaintId);
        }

        public async Task<Result> MarkComplaintIrrelevantAsync(int complaintId, MarkIrrelevantRequest request)
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

            var currentStatus = (ComplaintStatus)complaint.Status;
            if (IsLockedComplaint(currentStatus, complaint.IsDuplicate))
            {
                return Result.Failure("This complaint has been finalized and can no longer be modified.");
            }

            var reason = request.Reason.Trim();
            var now = DateTime.UtcNow;
            var updates = new List<ComplaintUpdate>
            {
                new()
                {
                    ComplaintId = complaintId,
                    UpdatedByUserId = userId.Value,
                    UpdateType = ComplaintUpdateType.MarkedIrrelevant,
                    OldStatus = currentStatus,
                    NewStatus = ComplaintStatus.Irrelevant,
                    NoteText = reason,
                    CreatedAt = now
                }
            };

            var updated = await _complaintRepository.ApplyAdminUpdateAsync(
                complaintId,
                (int)ComplaintStatus.Irrelevant,
                false,
                now,
                updates);
            if (!updated)
            {
                return Result.Failure("Complaint update failed. Please try again.");
            }

            _logger.LogInformation("Complaint {ComplaintId} marked irrelevant by admin", complaintId);

            return await GetComplaintByIdAsync(complaintId);
        }

        private static bool IsLockedComplaint(ComplaintStatus status, bool isDuplicate)
        {
            return status is ComplaintStatus.Resolved or ComplaintStatus.Irrelevant || isDuplicate;
        }

        private static bool IsAllowedStatusTransition(ComplaintStatus currentStatus, ComplaintStatus targetStatus)
        {
            return currentStatus switch
            {
                ComplaintStatus.Pending => targetStatus is ComplaintStatus.Pending or ComplaintStatus.InProgress,
                ComplaintStatus.InProgress => targetStatus is ComplaintStatus.InProgress or ComplaintStatus.Resolved,
                _ => false
            };
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
