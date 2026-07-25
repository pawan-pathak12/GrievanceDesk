import { useCallback, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { adminNoticeApi } from '../../api/noticeApi';
import NoticePriorityBadge from '../../components/common/NoticePriorityBadge';
import NoticePublicationBadge from '../../components/common/NoticePublicationBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { formatDate, formatTime } from '../../utils/enums';
import { Eye, EyeOff, Pencil, Send, Trash2 } from 'lucide-react';
import TableActionButton from '../../components/common/TableActionButton';

export default function AdminNoticesPage() {
  const [notices, setNotices] = useState([]);
  const [loading, setLoading] = useState(true);
  const [noticeToDelete, setNoticeToDelete] = useState(null);
  const [noticeToUnpublish, setNoticeToUnpublish] = useState(null);
  const [actionId, setActionId] = useState(null);
  const navigate = useNavigate();
  const toast = useToast();

  const loadNotices = useCallback(async () => {
    setLoading(true);
    const result = await adminNoticeApi.getAll();
    if (result.isSuccess) setNotices(result.data || []);
    else toast.error(result.errors?.[0] || 'Failed to load notices');
    setLoading(false);
  }, [toast]);

  useEffect(() => {
    loadNotices();
  }, [loadNotices]);

  async function changePublication(notice) {
    setActionId(notice.id);
    const result = notice.isPublished
      ? await adminNoticeApi.unpublish(notice.id)
      : await adminNoticeApi.publish(notice.id);

    if (result.isSuccess) {
      toast.success(notice.isPublished ? 'Notice unpublished' : 'Notice published');
      await loadNotices();
    } else {
      toast.error(result.errors?.[0] || 'Unable to update notice status');
    }
    setActionId(null);
    return result.isSuccess;
  }

  async function confirmUnpublish() {
    if (!noticeToUnpublish) return;
    const isSuccess = await changePublication(noticeToUnpublish);
    if (isSuccess) setNoticeToUnpublish(null);
  }

  async function archiveNotice() {
    if (!noticeToDelete) return;
    setActionId(noticeToDelete.id);
    const result = await adminNoticeApi.delete(noticeToDelete.id);
    if (result.isSuccess) {
      toast.success('Notice archived');
      setNoticeToDelete(null);
      await loadNotices();
    } else {
      toast.error(result.errors?.[0] || 'Unable to archive notice');
    }
    setActionId(null);
  }

  return (
    <div>
      <div className="page-header page-header-row">
        <div>
          <h1>Notices</h1>
          <p>Create, publish, and manage organization-wide updates.</p>
        </div>
        <button className="btn-submit" onClick={() => navigate(ROUTES.ADMIN_NOTICE_NEW)}>
          Create Notice
        </button>
      </div>

      {loading ? <LoadingSpinner /> : (
        <div className="data-table-wrapper">
          <div className="table-header">
            <div>
              <h2>All Notices</h2>
              <span>Showing {notices.length} active notices</span>
            </div>
          </div>
          {notices.length === 0 ? (
            <div className="empty-state">
              <strong>No notices yet</strong>
              <p>Create a draft when your organization has an update to share.</p>
            </div>
          ) : (
            <div className="table-scroll">
              <table className="data-table notice-table">
                <thead>
                  <tr>
                    <th>Title</th>
                    <th>Priority</th>
                    <th>Status</th>
                    <th>Created</th>
                    <th>Created Time</th>
                    <th>Updated</th>
                    <th>Updated Time</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {notices.map((notice) => (
                    <tr key={notice.id}>
                      <td className="table-title">{notice.title}</td>
                      <td><NoticePriorityBadge priority={notice.priority} /></td>
                      <td><NoticePublicationBadge isPublished={notice.isPublished} /></td>
                      <td>{formatDate(notice.createdAt)}</td>
                      <td>{formatTime(notice.createdAt)}</td>
                      <td>{formatDate(notice.updatedAt)}</td>
                      <td>{formatTime(notice.updatedAt)}</td>
                      <td>
                        <div className="notice-row-actions">
                          <TableActionButton label="View notice" icon={Eye} onClick={() => navigate(ROUTES.ADMIN_NOTICE_DETAIL.replace(':id', notice.id))} />
                          <TableActionButton label="Edit notice" icon={Pencil} onClick={() => navigate(ROUTES.ADMIN_NOTICE_EDIT.replace(':id', notice.id))} />
                          {notice.isPublished ? (
                            <TableActionButton label="Unpublish notice" icon={EyeOff} tone="danger" disabled={actionId === notice.id} onClick={() => setNoticeToUnpublish(notice)} />
                          ) : (
                            <TableActionButton label="Publish notice" icon={Send} tone="success" disabled={actionId === notice.id} onClick={() => changePublication(notice)} />
                          )}
                          <TableActionButton label="Archive notice" icon={Trash2} tone="danger" disabled={actionId === notice.id} onClick={() => setNoticeToDelete(notice)} />
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {noticeToUnpublish && (
        <div className="notice-modal-backdrop" onMouseDown={(event) => event.target === event.currentTarget && setNoticeToUnpublish(null)}>
          <section className="notice-dialog" role="dialog" aria-modal="true" aria-labelledby="unpublish-notice-title">
            <h2 id="unpublish-notice-title">Unpublish notice?</h2>
            <p>Do you want to unpublish this notice? It will no longer be visible to organization members.</p>
            <div className="notice-dialog-actions">
              <button className="btn-cancel" disabled={actionId === noticeToUnpublish.id} onClick={() => setNoticeToUnpublish(null)}>Cancel</button>
              <button className="btn-danger" disabled={actionId === noticeToUnpublish.id} onClick={confirmUnpublish}>Unpublish Notice</button>
            </div>
          </section>
        </div>
      )}

      {noticeToDelete && (
        <div className="notice-modal-backdrop" onMouseDown={(event) => event.target === event.currentTarget && setNoticeToDelete(null)}>
          <section className="notice-dialog" role="dialog" aria-modal="true" aria-labelledby="archive-notice-title">
            <h2 id="archive-notice-title">Archive notice?</h2>
            <p>This action will archive the notice. It will no longer appear for administrators or members.</p>
            <div className="notice-dialog-actions">
              <button className="btn-cancel" disabled={actionId === noticeToDelete.id} onClick={() => setNoticeToDelete(null)}>Cancel</button>
              <button className="btn-danger" disabled={actionId === noticeToDelete.id} onClick={archiveNotice}>Archive Notice</button>
            </div>
          </section>
        </div>
      )}
    </div>
  );
}
