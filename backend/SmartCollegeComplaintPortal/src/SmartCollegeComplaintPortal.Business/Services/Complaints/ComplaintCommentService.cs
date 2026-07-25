using Microsoft.Extensions.Logging;
using SmartCollegeComplaintPortal.Business.DTOs.Complaints;
using SmartCollegeComplaintPortal.Business.Interfaces.Auths;
using SmartCollegeComplaintPortal.Business.Interfaces.Complaints;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Data.QueryResult;
using SmartCollegeComplaintPortal.Domain.Commons.Pagination;
using SmartCollegeComplaintPortal.Domain.Entities.Complaints;
using SmartCollegeComplaintPortal.Domain.Enums;

namespace SmartCollegeComplaintPortal.Business.Services.Complaints
{
    public class ComplaintCommentService : IComplaintCommentService
    {
        private const int MaximumCommentLength = 1000;
        private readonly IComplaintCommentRepository _commentRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<ComplaintCommentService> _logger;

        public ComplaintCommentService(
            IComplaintCommentRepository commentRepository,
            IComplaintRepository complaintRepository,
            ICurrentUserService currentUserService,
            ILogger<ComplaintCommentService> logger)
        {
            _commentRepository = commentRepository;
            _complaintRepository = complaintRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> GetByComplaintIdAsync(int complaintId)
        {
            var access = await EnsureComplaintAccessAsync(complaintId);
            if (access != null) return access;

            var comments = await _commentRepository.GetByComplaintIdAsync(complaintId);
            return Result.Success(comments.Select(ToResponse).ToList());
        }

        public async Task<Result> CreateAsync(int complaintId, CreateComplaintCommentRequest request)
        {
            var access = await EnsureComplaintAccessAsync(complaintId);
            if (access != null) return access;

            var validation = ValidateComment(request.Comment);
            if (validation != null) return Result.Failure(validation);

            var userId = _currentUserService.GetUserId()!.Value;
            var comment = new ComplaintComment
            {
                ComplaintId = complaintId,
                UserId = userId,
                Comment = request.Comment.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var commentId = await _commentRepository.CreateAsync(comment);
            if (commentId <= 0) return Result.Failure("Comment could not be added.");

            _logger.LogInformation("User {UserId} added comment {CommentId} to complaint {ComplaintId}", userId, commentId, complaintId);
            var created = await _commentRepository.GetByIdAsync(commentId);
            return created == null
                ? Result.Failure("Comment was added but could not be loaded.")
                : Result.Success(ToResponse(created), "Comment added successfully.");
        }

        public async Task<Result> UpdateAsync(int commentId, UpdateComplaintCommentRequest request)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return Result.Unauthorized("User could not be identified.");

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) return Result.NotFound("Comment not found.");
            if (comment.UserId != userId.Value) return Result.Forbidden("You can edit only your own comments.");

            var validation = ValidateComment(request.Comment);
            if (validation != null) return Result.Failure(validation);

            var updated = await _commentRepository.UpdateAsync(commentId, request.Comment.Trim(), DateTime.UtcNow);
            if (!updated) return Result.Failure("Comment could not be updated.");

            var response = await _commentRepository.GetByIdAsync(commentId);
            return response == null
                ? Result.Failure("Comment was updated but could not be loaded.")
                : Result.Success(ToResponse(response), "Comment updated successfully.");
        }

        public async Task<Result> DeleteOwnAsync(int commentId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return Result.Unauthorized("User could not be identified.");

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) return Result.NotFound("Comment not found.");
            if (comment.UserId != userId.Value) return Result.Forbidden("You can delete only your own comments.");

            return await DeleteAsync(commentId);
        }

        public async Task<Result> DeleteAsAdminAsync(int commentId)
        {
            if (await _commentRepository.GetByIdAsync(commentId) == null) return Result.NotFound("Comment not found.");
            return await DeleteAsync(commentId);
        }

        private async Task<Result?> EnsureComplaintAccessAsync(int complaintId)
        {
            var userId = _currentUserService.GetUserId();
            if (userId == null) return Result.Unauthorized("User could not be identified.");
            if (!await _complaintRepository.ExistsAsync(complaintId)) return Result.NotFound($"Complaint with ID {complaintId} not found.");

            return null;
        }

        private async Task<Result> DeleteAsync(int commentId)
        {
            var deleted = await _commentRepository.SoftDeleteAsync(commentId, DateTime.UtcNow);
            return deleted ? Result.Success("Comment deleted successfully.") : Result.Failure("Comment could not be deleted.");
        }

        private static string? ValidateComment(string? comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return "Comment is required.";
            return comment.Trim().Length > MaximumCommentLength ? $"Comment cannot exceed {MaximumCommentLength} characters." : null;
        }

        private static ComplaintCommentResponse ToResponse(ComplaintCommentResult comment)
        {
            return new ComplaintCommentResponse
            {
                ComplaintCommentId = comment.ComplaintCommentId,
                ComplaintId = comment.ComplaintId,
                UserId = comment.UserId,
                Comment = comment.Comment,
                IsEdited = comment.IsEdited,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                UserName = comment.UserName,
                UserRole = Enum.IsDefined(typeof(UserRole), comment.UserRole)
                    ? ((UserRole)comment.UserRole).ToString()
                    : UserRole.User.ToString()
            };
        }
    }
}
