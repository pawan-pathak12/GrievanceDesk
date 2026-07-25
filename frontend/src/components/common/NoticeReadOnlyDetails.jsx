import NoticePublicationBadge from './NoticePublicationBadge';
import { formatDateTime } from '../../utils/enums';

function getContentRows(content) {
  const lines = String(content || '').split(/\r?\n/);
  const wrappedLines = lines.reduce((total, line) => total + Math.max(1, Math.ceil(line.length / 76)), 0);
  return Math.max(4, wrappedLines);
}

export default function NoticeReadOnlyDetails({ notice, isPublished = notice.isPublished }) {
  return (
    <>
      <div className="notice-readonly-form" aria-label="Notice details">
        <div className="form-group">
          <label htmlFor="readonly-notice-title">Title</label>
          <input id="readonly-notice-title" value={notice.title} disabled />
        </div>
        <div className="form-group">
          <label htmlFor="readonly-notice-priority">Priority</label>
          <select id="readonly-notice-priority" value={notice.priority} disabled>
            <option value="Normal">Normal</option>
            <option value="Important">Important</option>
            <option value="Urgent">Urgent</option>
          </select>
        </div>
        <div className="form-group">
          <label htmlFor="readonly-notice-content">Content</label>
          <textarea id="readonly-notice-content" value={notice.content} rows={getContentRows(notice.content)} disabled />
        </div>
      </div>

      <section className="notice-information-section" aria-labelledby="notice-information-title">
        <h2 id="notice-information-title">Notice Information</h2>
        <dl className="notice-meta-row">
          {notice.createdAt && <div><dt>Created</dt><dd>{formatDateTime(notice.createdAt)}</dd></div>}
          {notice.updatedAt && <div><dt>Last Updated</dt><dd>{formatDateTime(notice.updatedAt)}</dd></div>}
          <div><dt>Status</dt><dd><NoticePublicationBadge isPublished={isPublished} /></dd></div>
          {isPublished && notice.publishedAt && <div><dt>Published</dt><dd>{formatDateTime(notice.publishedAt)}</dd></div>}
        </dl>
      </section>
    </>
  );
}
