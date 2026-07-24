import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { ComplaintStatus, formatDateTime, formatStatus, normalizeStatus } from '../../utils/enums';

const EMPTY_FORM = {
  status: ComplaintStatus.Pending,
  note: '',
  isDuplicate: false,
  isIrrelevant: false,
  irrelevantReason: '',
};

function renderUpdate(update) {
  if (update.updateType === 'Created') return 'Complaint created';
  if (update.updateType === 'StatusChanged') {
    return `${formatStatus(update.oldStatus)} to ${formatStatus(update.newStatus)}`;
  }
  if (update.updateType === 'MarkedDuplicate') return 'Marked as duplicate';
  if (update.updateType === 'MarkedIrrelevant') return update.noteText || 'Marked as irrelevant';
  if (update.noteText) return update.noteText;
  return 'Complaint activity recorded';
}

function getAllowedStatuses(status) {
  if (status === ComplaintStatus.Pending) {
    return [ComplaintStatus.Pending, ComplaintStatus.InProgress];
  }

  if (status === ComplaintStatus.InProgress) {
    return [ComplaintStatus.InProgress, ComplaintStatus.Resolved];
  }

  return [status];
}

export default function AdminComplaintDetailPage() {
  const { id } = useParams();
  const [complaint, setComplaint] = useState(null);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState(EMPTY_FORM);
  const [formError, setFormError] = useState('');
  const [updating, setUpdating] = useState(false);
  const toast = useToast();

  useEffect(() => {
    loadComplaint();
  }, [id]);

  useEffect(() => {
    if (!complaint) return;

    const status = normalizeStatus(complaint.status);
    setForm({
      ...EMPTY_FORM,
      status,
      isDuplicate: Boolean(complaint.isDuplicate),
      isIrrelevant: status === ComplaintStatus.Irrelevant,
    });
    setFormError('');
  }, [complaint]);

  async function loadComplaint() {
    setLoading(true);
    const res = await adminApi.getById(id);
    if (res.isSuccess) setComplaint(res.data);
    else toast.error(res.errors?.[0] || 'Failed to load complaint');
    setLoading(false);
  }

  function updateForm(field, value) {
    setForm((current) => ({ ...current, [field]: value }));
    if (formError) setFormError('');
  }

  function toggleDuplicate(checked) {
    setForm((current) => ({
      ...current,
      isDuplicate: checked,
      isIrrelevant: checked ? false : current.isIrrelevant,
      irrelevantReason: checked ? '' : current.irrelevantReason,
    }));
    if (formError) setFormError('');
  }

  function toggleIrrelevant(checked) {
    setForm((current) => ({
      ...current,
      isIrrelevant: checked,
      isDuplicate: checked ? false : current.isDuplicate,
    }));
    if (formError) setFormError('');
  }

  async function handleSubmit(event) {
    event.preventDefault();
    const selectedStatus = Number(form.status);
    const note = form.note.trim();
    const irrelevantReason = form.irrelevantReason.trim();

    if (form.isDuplicate && form.isIrrelevant) {
      setFormError('A complaint cannot be marked as both duplicate and irrelevant.');
      return;
    }

    if (selectedStatus === ComplaintStatus.Resolved && !note) {
      setFormError('A resolution note is required before resolving a complaint.');
      return;
    }

    if (form.isIrrelevant && !irrelevantReason) {
      setFormError('An irrelevant reason is required when marking a complaint irrelevant.');
      return;
    }

    if (
      selectedStatus === normalizeStatus(complaint.status) &&
      !note &&
      !form.isDuplicate &&
      !form.isIrrelevant
    ) {
      setFormError('Select a new status, add a note, or mark the complaint before updating.');
      return;
    }

    setUpdating(true);
    setFormError('');
    const res = await adminApi.updateComplaint(id, {
      status: selectedStatus,
      note: note || null,
      isDuplicate: form.isDuplicate,
      isIrrelevant: form.isIrrelevant,
      irrelevantReason: irrelevantReason || null,
    });
    setUpdating(false);

    if (res.isSuccess) {
      setComplaint(res.data);
      toast.success('Complaint updated successfully.');
    } else {
      const error = res.errors?.[0] || 'Unable to update complaint.';
      setFormError(error);
      toast.error(error);
    }
  }

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="empty-state"><strong>Complaint not found</strong></div>;

  const currentStatus = normalizeStatus(complaint.status);
  const isLocked = complaint.isDuplicate || currentStatus === ComplaintStatus.Resolved || currentStatus === ComplaintStatus.Irrelevant;
  const allowedStatuses = getAllowedStatuses(currentStatus);

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

        <div className="complaint-detail-grid">
          <div className="detail-section complaint-description">
            <div className="detail-label">Description</div>
            <p>{complaint.description}</p>
          </div>
          <div className="detail-section">
            <div className="detail-label">Category</div>
            <p>{complaint.category}</p>
          </div>
          <div className="detail-section">
            <div className="detail-label">Current Status</div>
            <p>{formatStatus(complaint.status)}</p>
          </div>
        </div>
      </div>

      <div className="detail-card complaint-update-card">
        <div className="complaint-update-heading">
          <div>
            <h2>Update Complaint</h2>
            <p>Review and submit all changes together.</p>
          </div>
        </div>

        {isLocked && (
          <div className="complaint-finalized-message" role="status">
            This complaint has been finalized and can no longer be modified.
          </div>
        )}

        <form className="complaint-update-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="complaint-status">Status</label>
            <select
              id="complaint-status"
              value={form.status}
              disabled={isLocked || updating}
              onChange={(event) => updateForm('status', Number(event.target.value))}
            >
              {allowedStatuses.map((status) => (
                <option key={status} value={status}>{formatStatus(status)}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="resolution-note">Resolution Note</label>
            <textarea
              id="resolution-note"
              value={form.note}
              disabled={isLocked || updating}
              maxLength={2000}
              placeholder="Add an internal resolution note"
              onChange={(event) => updateForm('note', event.target.value)}
            />
          </div>

          <div className="complaint-update-options">
            <label className="update-checkbox">
              <input
                type="checkbox"
                checked={form.isDuplicate}
                disabled={isLocked || updating}
                onChange={(event) => toggleDuplicate(event.target.checked)}
              />
              <span>Mark as Duplicate</span>
            </label>
            <label className="update-checkbox">
              <input
                type="checkbox"
                checked={form.isIrrelevant}
                disabled={isLocked || updating}
                onChange={(event) => toggleIrrelevant(event.target.checked)}
              />
              <span>Mark as Irrelevant</span>
            </label>
          </div>

          {form.isIrrelevant && (
            <div className="form-group">
              <label htmlFor="irrelevant-reason">Irrelevant Reason</label>
              <textarea
                id="irrelevant-reason"
                value={form.irrelevantReason}
                disabled={isLocked || updating}
                maxLength={2000}
                placeholder="Explain why this complaint is irrelevant"
                onChange={(event) => updateForm('irrelevantReason', event.target.value)}
              />
            </div>
          )}

          {formError && <p className="complaint-update-error" role="alert">{formError}</p>}

          <div className="complaint-update-footer">
            <button type="submit" className="btn-submit" disabled={isLocked || updating}>
              {updating ? 'Updating...' : 'Update Complaint'}
            </button>
          </div>
        </form>
      </div>

      <div className="detail-card">
        <h2>Activity Timeline</h2>
        {(!complaint.updates || complaint.updates.length === 0) ? (
          <p className="muted-text">No activity has been recorded yet.</p>
        ) : (
          <div className="timeline">
            {complaint.updates.map((update, index) => (
              <div className="timeline-item" key={update.id || index}>
                <div className="timeline-item-header">
                  <span className="timeline-item-type">{update.updateType || 'Update'}</span>
                  <span className="timeline-item-date">{formatDateTime(update.createdAt)}</span>
                </div>
                <div className="timeline-item-body">{renderUpdate(update)}</div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
