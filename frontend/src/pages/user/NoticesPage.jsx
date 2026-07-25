import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { noticeApi } from '../../api/noticeApi';
import NoticePriorityBadge from '../../components/common/NoticePriorityBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';
import { formatDateTime } from '../../utils/enums';
import { markItemsViewed } from '../../utils/viewTracking';
import { Eye } from 'lucide-react';
import TableActionButton from '../../components/common/TableActionButton';

function getPreview(content) {
  const compact = String(content || '').replace(/\s+/g, ' ').trim();
  return compact.length > 120 ? `${compact.slice(0, 120)}...` : compact;
}

export default function NoticesPage() {
  const [notices, setNotices] = useState([]);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(true);
  const toast = useToast();
  const navigate = useNavigate();
  const { user } = useAuth();
  const userKey = user?.userId || user?.id || user?.email || 'member';

  const loadNotices = useCallback(async () => {
    setLoading(true);
    const result = await noticeApi.getPublished();
    if (result.isSuccess) {
      const items = result.data || [];
      setNotices(items);
      markItemsViewed(userKey, 'notices', items);
    }
    else toast.error(result.errors?.[0] || 'Failed to load notices');
    setLoading(false);
  }, [toast, userKey]);

  useEffect(() => { loadNotices(); }, [loadNotices]);

  const visibleNotices = useMemo(() => {
    const term = query.trim().toLowerCase();
    if (!term) return notices;
    return notices.filter((notice) => `${notice.title} ${notice.content}`.toLowerCase().includes(term));
  }, [notices, query]);

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header page-header-row notice-page-header">
        <div>
          <h1>Notices</h1>
          <p>Read the latest updates shared by your organization.</p>
        </div>
        <div className="notice-search"><label htmlFor="notice-search">Search notices</label><input id="notice-search" value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Search by title or content" /></div>
      </div>

      {visibleNotices.length === 0 ? (
        <div className="empty-state notice-empty-state">
          <strong>{notices.length ? 'No notices match your search' : 'No published notices yet'}</strong>
          <p>{notices.length ? 'Try a different search term.' : 'Published organization updates will appear here.'}</p>
        </div>
      ) : (
        <div className="notice-list-grid">
          {visibleNotices.map((notice) => (
            <article className="notice-list-card" key={notice.id}>
              <div className="notice-card-header">
                <NoticePriorityBadge priority={notice.priority} />
                <TableActionButton label="View notice" icon={Eye} onClick={() => navigate(ROUTES.NOTICE_DETAIL.replace(':id', notice.id))} />
              </div>
              <h2>{notice.title}</h2>
              <p>{getPreview(notice.content)}</p>
              <div className="notice-card-footer"><span>{formatDateTime(notice.publishedAt)}</span></div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}
