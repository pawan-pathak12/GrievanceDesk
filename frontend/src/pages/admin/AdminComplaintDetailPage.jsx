import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { ComplaintStatus, StatusOptions, formatDateTime, formatStatus, normalizeStatus } from '../../utils/enums';

function renderUpdate(update) {
  if (update.updateType === 'StatusChanged') {
    return `${formatStatus(update.oldStatus)} to ${formatStatus(update.newStatus)}`;
  }
  if (update.noteText) return update.noteText;
  return 'Complaint activity recorded';
}

export default function AdminComplaintDetailPage() {
  const { id } = useParams();
  const [complaint, setComplaint] = useState(null);
  const [loading, setLoading] = useState(true);
  const [noteText, setNoteText] = useState('');
  const [irrelevantReason, setIrrelevantReason] = useState('');
  const [actionLoading, setActionLoading] = useState(false);
  const toast = useToast();

  useEffect(() => {
    loadComplaint();
  }, [id]);

  async function loadComplaint() {
    setLoading(true);
    const res = await adminApi.getById(id);
    if (res.isSuccess) setComplaint(res.data);
    else toast.error(res.errors?.[0] || 'Failed to load complaint');
    setLoading(false);
  }

  async function runAction(action, successMessage) {
    setActionLoading(true);
    const res = await action();
    setActionLoading(false);
    if (res.isSuccess) {
      toast.success(successMessage);
      loadComplaint();
      return true;
    }
    toast.error(res.errors?.[0] || 'Action failed');
    return false;
  }

  async function handleStatusChange(newStatus) {
    await runAction(() => adminApi.updateStatus(id, newStatus), 'Status updated successfully');
  }

  async function handleAddNote(e) {
    e.preventDefault();
    if (!noteText.trim()) return;
    const ok = await runAction(() => adminApi.addNote(id, noteText.trim()), 'Note added successfully');
    if (ok) setNoteText('');
  }

  async function handleMarkDuplicate() {
    await runAction(() => adminApi.markDuplicate(id, true), 'Marked as duplicate');
  }

  async function handleMarkIrrelevant(e) {
    e.preventDefault();
    const ok = await runAction(
      () => adminApi.markIrrelevant(id, irrelevantReason.trim() || 'Marked irrelevant by administrator'),
      'Marked as irrelevant'
    );
    if (ok) setIrrelevantReason('');
  }

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="empty-state"><strong>Complaint not found</strong></div>;

  const currentStatus = normalizeStatus(complaint.status);

  return (
    <div>
      <Link to={ROUTES.ADMIN_COMPLAINTS} className="back-link">Back to All Complaints</Link>

      <div className="detail-card detail-hero">
        <div className="detail-heading">
          <div>
            <span className="eyebrow">Case #{String(complaint.id).padStart(3, '0')}</span>
            <h1>{complaint.title}</h1>
            <div className="badge-row">
              <CategoryBadge category={complaint.category} />
              <StatusBadge status={complaint.status} />
              {complaint.isDuplicate && <span className="badge badge-warning">Duplicate</span>}
              {complaint.isEdited && <span className="badge badge-warning">Edited</span>}
            </div>
          </div>
          <div className="detail-meta">
            <span>Created</span>
            <strong>{formatDateTime(complaint.createdAt)}</strong>
          </div>
        </div>

        <div className="detail-section">
          <div className="detail-label">Description</div>
          <p>{complaint.description}</p>
        </div>

        <div className="admin-actions">
          <div className="action-group">
            <label>Change Status</label>
            <div className="action-buttons">
              {StatusOptions.filter((opt) => opt.value !== currentStatus && opt.value !== ComplaintStatus.Irrelevant).map((opt) => (
                <button key={opt.value} className="btn-action" onClick={() => handleStatusChange(opt.value)} disabled={actionLoading}>
                  {opt.label}
                </button>
              ))}
            </div>
          </div>
          <button className="btn-action" onClick={handleMarkDuplicate} disabled={actionLoading || complaint.isDuplicate}>
            Mark Duplicate
          </button>
        </div>

        <form className="note-form" onSubmit={handleAddNote}>
          <input
            type="text"
            placeholder="Add an internal resolution note"
            value={noteText}
            onChange={(e) => setNoteText(e.target.value)}
            minLength={3}
            maxLength={2000}
          />
          <button type="submit" disabled={actionLoading || !noteText.trim()}>Add Note</button>
        </form>

        <form className="note-form danger-form" onSubmit={handleMarkIrrelevant}>
          <input
            type="text"
            placeholder="Reason for marking irrelevant"
            value={irrelevantReason}
            onChange={(e) => setIrrelevantReason(e.target.value)}
            maxLength={2000}
          />
          <button type="submit" disabled={actionLoading || currentStatus === ComplaintStatus.Irrelevant}>
            Mark Irrelevant
          </button>
        </form>
      </div>

      <div className="detail-card">
        <h2>Activity Timeline</h2>
        {(!complaint.updates || complaint.updates.length === 0) ? (
          <p className="muted-text">No activity has been recorded yet.</p>
        ) : (
          <div className="timeline">
            {complaint.updates.map((u, i) => (
              <div className="timeline-item" key={u.id || i}>
                <div className="timeline-item-header">
                  <span className="timeline-item-type">{u.updateType || 'Update'}</span>
                  <span className="timeline-item-date">{formatDateTime(u.createdAt)}</span>
                </div>
                <div className="timeline-item-body">{renderUpdate(u)}</div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
