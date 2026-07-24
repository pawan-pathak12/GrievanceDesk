import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { StatusLabels, ComplaintStatus } from '../../utils/enums';

export default function AdminComplaintDetailPage() {
  const { id } = useParams();
  const [complaint, setComplaint] = useState(null);
  const [loading, setLoading] = useState(true);
  const [noteText, setNoteText] = useState('');
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

  async function handleStatusChange(newStatus) {
    setActionLoading(true);
    const res = await adminApi.updateStatus(id, newStatus);
    setActionLoading(false);
    if (res.isSuccess) {
      toast.success('Status updated successfully');
      loadComplaint();
    } else {
      toast.error(res.errors?.[0] || 'Failed to update status');
    }
  }

  async function handleAddNote(e) {
    e.preventDefault();
    if (!noteText.trim()) return;
    setActionLoading(true);
    const res = await adminApi.addNote(id, noteText.trim());
    setActionLoading(false);
    if (res.isSuccess) {
      toast.success('Note added successfully');
      setNoteText('');
      loadComplaint();
    } else {
      toast.error(res.errors?.[0] || 'Failed to add note');
    }
  }

  async function handleMarkDuplicate() {
    setActionLoading(true);
    const res = await adminApi.markDuplicate(id, true);
    setActionLoading(false);
    if (res.isSuccess) {
      toast.success('Marked as duplicate');
      loadComplaint();
    } else {
      toast.error(res.errors?.[0] || 'Failed to mark duplicate');
    }
  }

  async function handleMarkIrrelevant() {
    const reason = prompt('Enter reason for marking as irrelevant:');
    if (!reason) return;
    setActionLoading(true);
    const res = await adminApi.markIrrelevant(id, reason);
    setActionLoading(false);
    if (res.isSuccess) {
      toast.success('Marked as irrelevant');
      loadComplaint();
    } else {
      toast.error(res.errors?.[0] || 'Failed to mark irrelevant');
    }
  }

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="empty-state"><p>Complaint not found.</p></div>;

  return (
    <div>
      <Link to={ROUTES.ADMIN_COMPLAINTS} className="back-link">← Back to All Complaints</Link>

      <div className="detail-card">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 16 }}>
          <div>
            <h2>#{String(complaint.id).padStart(3, '0')} — {complaint.title}</h2>
            <div style={{ display: 'flex', gap: 8, marginTop: 8 }}>
              <CategoryBadge category={complaint.category} />
              <StatusBadge status={complaint.status} />
              {complaint.isDuplicate && <span className="badge" style={{ background: '#FEF3C7', color: '#92400E' }}>Duplicate</span>}
            </div>
          </div>
          <span style={{ fontSize: '0.8rem', color: 'var(--text-light)' }}>
            {new Date(complaint.createdAt).toLocaleString()}
          </span>
        </div>

        <div style={{ marginTop: 16 }}>
          <div className="detail-label">Description</div>
          <p style={{ marginTop: 4, lineHeight: 1.7 }}>{complaint.description}</p>
        </div>

        {/* Admin Actions */}
        <div className="admin-actions">
          <label style={{ fontSize: '0.8rem', fontWeight: 600, marginRight: 8, alignSelf: 'center' }}>Change Status:</label>
          {Object.entries(ComplaintStatus).filter(([, v]) => v !== complaint.status).map(([label, val]) => (
            <button key={val} className="btn-action" onClick={() => handleStatusChange(val)} disabled={actionLoading}>
              → {label}
            </button>
          ))}
          <button className="btn-action" onClick={handleMarkDuplicate} disabled={actionLoading || complaint.isDuplicate}>
            🔁 Duplicate
          </button>
          <button className="btn-action danger" onClick={handleMarkIrrelevant} disabled={actionLoading}>
            ⛔ Irrelevant
          </button>
        </div>

        {/* Add Note */}
        <form className="note-form" onSubmit={handleAddNote}>
          <input
            type="text"
            placeholder="Add a resolution note..."
            value={noteText}
            onChange={(e) => setNoteText(e.target.value)}
          />
          <button type="submit" disabled={actionLoading || !noteText.trim()}>Add Note</button>
        </form>
      </div>

      {/* Timeline */}
      <div className="detail-card">
        <h2>Activity Timeline</h2>
        {(!complaint.updates || complaint.updates.length === 0) ? (
          <p style={{ color: 'var(--text-light)', marginTop: 8 }}>No activity yet.</p>
        ) : (
          <div className="timeline" style={{ marginTop: 16 }}>
            {complaint.updates.map((u, i) => (
              <div className="timeline-item" key={u.id || i}>
                <div className="timeline-item-header">
                  <span className="timeline-item-type">
                    {u.updateType === 0 && 'Status Changed'}
                    {u.updateType === 1 && '📝 Note Added'}
                    {u.updateType === 2 && 'Marked as Duplicate'}
                    {u.updateType === 3 && 'Marked as Irrelevant'}
                    {u.updateType === 4 && 'Complaint Edited'}
                    {u.updateType !== 0 && u.updateType !== 1 && u.updateType !== 2 && u.updateType !== 3 && u.updateType !== 4 && `Update (${u.updateType})`}
                  </span>
                  <span className="timeline-item-date">
                    {new Date(u.createdAt).toLocaleString()}
                  </span>
                </div>
                <div className="timeline-item-body">
                  {u.updateType === 0 && (
                    <span>{StatusLabels[u.oldStatus] || '—'} → {StatusLabels[u.newStatus] || '—'}</span>
                  )}
                  {u.noteText && <p>{u.noteText}</p>}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
