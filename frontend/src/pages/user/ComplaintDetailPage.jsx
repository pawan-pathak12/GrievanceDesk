import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { complaintApi } from '../../api/complaintApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { formatDateTime, formatStatus } from '../../utils/enums';

function renderUpdate(update) {
  if (update.updateType === 'StatusChanged') {
    return `${formatStatus(update.oldStatus)} to ${formatStatus(update.newStatus)}`;
  }
  if (update.noteText) return update.noteText;
  return 'Complaint activity recorded';
}

export default function ComplaintDetailPage() {
  const { id } = useParams();
  const [complaint, setComplaint] = useState(null);
  const [loading, setLoading] = useState(true);
  const toast = useToast();

  useEffect(() => {
    loadComplaint();
  }, [id]);

  async function loadComplaint() {
    setLoading(true);
    const res = await complaintApi.getById(id);
    if (res.isSuccess) setComplaint(res.data);
    else toast.error(res.errors?.[0] || 'Failed to load complaint');
    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="empty-state"><strong>Complaint not found</strong></div>;

  return (
    <div>
      <Link to={ROUTES.MY_COMPLAINTS} className="back-link">Back to My Complaints</Link>

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
