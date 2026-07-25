using Microsoft.EntityFrameworkCore;
using SmartCollegeComplaintPortal.Data.Database;
using SmartCollegeComplaintPortal.Data.Interfaces;
using SmartCollegeComplaintPortal.Domain.Entities.Notices;

namespace SmartCollegeComplaintPortal.Data.Repositories
{
    public class NoticeRepository : INoticeRepository
    {
        private readonly SystemEfCoreDbContext _dbContext;

        public NoticeRepository(SystemEfCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Notice> CreateAsync(Notice notice)
        {
            _dbContext.Notices.Add(notice);
            await _dbContext.SaveChangesAsync();
            return notice;
        }

        public async Task<Notice> UpdateAsync(Notice notice)
        {
            _dbContext.Notices.Update(notice);
            await _dbContext.SaveChangesAsync();
            return notice;
        }

        public async Task<bool> SoftDeleteAsync(int noticeId, int updatedBy, DateTime updatedAt)
        {
            var notice = await GetByIdAsync(noticeId);
            if (notice == null)
            {
                return false;
            }

            notice.IsDeleted = true;
            notice.UpdatedBy = updatedBy;
            notice.UpdatedAt = updatedAt;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<Notice?> GetByIdAsync(int noticeId)
        {
            return _dbContext.Notices.FirstOrDefaultAsync(notice => notice.Id == noticeId);
        }

        public async Task<IReadOnlyList<Notice>> GetAllAsync()
        {
            return await _dbContext.Notices
                .OrderByDescending(notice => notice.IsPublished)
                .ThenByDescending(notice => notice.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Notice>> GetPublishedAsync()
        {
            return await _dbContext.Notices
                .Where(notice => notice.IsPublished)
                .OrderByDescending(notice => notice.PublishedAt)
                .ThenByDescending(notice => notice.CreatedAt)
                .ToListAsync();
        }

        public Task<bool> ExistsAsync(int noticeId)
        {
            return _dbContext.Notices.AnyAsync(notice => notice.Id == noticeId);
        }
    }
}
