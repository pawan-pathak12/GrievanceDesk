import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { adminNoticeApi } from '../../api/noticeApi';
import NoticeReadOnlyDetails from '../../components/common/NoticeReadOnlyDetails';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';

export default function AdminNoticeDetailPage() {
  const { id } = useParams();
  const [notice, setNotice] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [isArchiveOpen, setIsArchiveOpen] = useState(false);
  const [isUnpublishOpen, setIsUnpublishOpen] = useState(false);
  const [isActionsOpen, setIsActionsOpen] = useState(false);
  const toast = useToast();
  const navigate = useNavigate();

  const loadNotice = useCallback(async () => {
    setLoading(true);
    const result = await adminNoticeApi.getById(id);
    if (result.isSuccess) setNotice(result.data);
    else {
      toast.error(result.errors?.[0] || 'Notice not found');
      navigate(ROUTES.ADMIN_NOTICES);
    }
    setLoading(false);
  }, [id, navigate, toast]);

  useEffect(() => { loadNotice(); }, [loadNotice]);

  async function publishNotice() {
    setSaving(true);
    const result = await adminNoticeApi.publish(id);
    if (result.isSuccess) {
      toast.success('Notice published');
      setNotice(result.data);
    } else toast.error(result.errors?.[0] || 'Unable to publish notice');
    setSaving(false);
  }

  async function unpublishNotice() {
    setSaving(true);
    const result = await adminNoticeApi.unpublish(id);
    if (result.isSuccess) {
      toast.success('Notice unpublished');
      setNotice(result.data);
      setIsUnpublishOpen(false);
    } else toast.error(result.errors?.[0] || 'Unable to unpublish notice');
    setSaving(false);
  }

  async function archiveNotice() {
    setSaving(true);
    const result = await adminNoticeApi.delete(id);
    if (result.isSuccess) {
      toast.success('Notice archived');
      navigate(ROUTES.ADMIN_NOTICES);
    } else {
      toast.error(result.errors?.[0] || 'Unable to archive notice');
      setSaving(false);
    }
  }

  if (loading) return <LoadingSpinner />;
  if (!notice) return null;

  return (
    <div className="notice-detail-page">
      <Link to={ROUTES.ADMIN_NOTICES} className="notice-breadcrumb"><span aria-hidden="true">&larr;</span> Notices</Link>
      <section className="detail-card notice-detail-card">
        <div className="detail-heading">
          <div>
            <span className="eyebrow">Organization Notice</span>
            <h1>Notice Details</h1>
          </div>
          <div className="notice-detail-actions">
            <button className="btn-outline" onClick={() => navigate(ROUTES.ADMIN_NOTICE_EDIT.replace(':id', id))}>Edit</button>
            <button className={notice.isPublished ? 'btn-danger' : 'btn-submit'} disabled={saving} onClick={() => notice.isPublished ? setIsUnpublishOpen(true) : publishNotice()}>
              {notice.isPublished ? 'Unpublish' : 'Publish'}
            </button>
            <div className="notice-actions-menu">
              <button
                type="button"
                className="notice-more-actions"
                title="More actions"
                aria-label="More actions"
                aria-expanded={isActionsOpen}
                onClick={() => setIsActionsOpen((open) => !open)}
              >
                ...
              </button>
              {isActionsOpen && (
                <div className="notice-actions-popover">
                  <button type="button" className="notice-archive-option" onClick={() => { setIsActionsOpen(false); setIsArchiveOpen(true); }}>Archive notice</button>
                </div>
              )}
            </div>
          </div>
        </div>
        <NoticeReadOnlyDetails notice={notice} />
      </section>

      {isUnpublishOpen && (
        <div className="notice-modal-backdrop" onMouseDown={(event) => event.target === event.currentTarget && setIsUnpublishOpen(false)}>
          <section className="notice-dialog" role="dialog" aria-modal="true" aria-labelledby="unpublish-notice-title">
            <h2 id="unpublish-notice-title">Unpublish notice?</h2>
            <p>Do you want to unpublish this notice? It will no longer be visible to organization members.</p>
            <div className="notice-dialog-actions">
              <button className="btn-cancel" disabled={saving} onClick={() => setIsUnpublishOpen(false)}>Cancel</button>
              <button className="btn-danger" disabled={saving} onClick={unpublishNotice}>Unpublish Notice</button>
            </div>
          </section>
        </div>
      )}

      {isArchiveOpen && (
        <div className="notice-modal-backdrop" onMouseDown={(event) => event.target === event.currentTarget && setIsArchiveOpen(false)}>
          <section className="notice-dialog" role="dialog" aria-modal="true" aria-labelledby="archive-detail-notice-title">
            <h2 id="archive-detail-notice-title">Archive notice?</h2>
            <p>This action will archive the notice. It will no longer appear for administrators or members.</p>
            <div className="notice-dialog-actions">
              <button className="btn-cancel" disabled={saving} onClick={() => setIsArchiveOpen(false)}>Cancel</button>
              <button className="btn-danger" disabled={saving} onClick={archiveNotice}>Archive Notice</button>
            </div>
          </section>
        </div>
      )}
    </div>
  );
}
