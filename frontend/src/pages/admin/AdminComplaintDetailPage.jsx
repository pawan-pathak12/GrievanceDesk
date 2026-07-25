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
};

function describeActivity(update) {
  const actor = update.updatedByName || 'System';

  if (update.updateType === 'Created') {
    return { title: 'Complaint Created', detail: `Created by ${actor}` };
  }

  if (update.updateType === 'StatusChanged') {
    return {
      title: 'Status Changed',
      detail: `${formatStatus(update.oldStatus)} -> ${formatStatus(update.newStatus)}`,
      actor: `Updated by ${actor}`,
    };
  }

  if (update.updateType === 'NoteAdded') {
    return { title: 'Internal Note', detail: update.noteText || 'Internal note added', actor: `Added by ${actor}` };
  }

  if (update.updateType === 'MarkedDuplicate') {
    return { title: 'Marked Duplicate', detail: `Marked by ${actor}` };
  }

  if (update.updateType === 'MarkedIrrelevant') {
    return { title: 'Marked Irrelevant', detail: update.noteText || 'Marked as irrelevant', actor: `Marked by ${actor}` };
  }

  return { title: 'Complaint Activity', detail: update.noteText || 'Complaint activity recorded', actor: `Updated by ${actor}` };
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
  const [irrelevantDialogOpen, setIrrelevantDialogOpen] = useState(false);
  const [irrelevantReason, setIrrelevantReason] = useState('');
  const [irrelevantError, setIrrelevantError] = useState('');
  const [markingIrrelevant, setMarkingIrrelevant] = useState(false);
  const toast = useToast();

  useEffect(() => {
    loadComplaint();
  }, [id]);

  useEffect(() => {
    if (!complaint) return;

    setForm({
      ...EMPTY_FORM,
      status: normalizeStatus(complaint.status),
      isDuplicate: Boolean(complaint.isDuplicate),
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

  async function handleSubmit(event) {
    event.preventDefault();
    const selectedStatus = Number(form.status);
    const note = form.note.trim();

    if (selectedStatus === ComplaintStatus.Resolved && !note) {
      setFormError('A resolution note is required before resolving a complaint.');
      return;
    }

    if (
      selectedStatus === normalizeStatus(complaint.status) &&
      !note &&
      !form.isDuplicate
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

  function openIrrelevantDialog() {
    setIrrelevantReason('');
    setIrrelevantError('');
    setIrrelevantDialogOpen(true);
  }

  async function handleMarkIrrelevant(event) {
    event.preventDefault();
    const reason = irrelevantReason.trim();

    if (reason.length < 3) {
      setIrrelevantError('Enter a reason of at least 3 characters.');
      return;
    }

    setMarkingIrrelevant(true);
    setIrrelevantError('');
    const res = await adminApi.markIrrelevant(id, reason);
    setMarkingIrrelevant(false);

    if (res.isSuccess) {
      setComplaint(res.data);
      setIrrelevantDialogOpen(false);
      toast.success('Complaint marked as irrelevant.');
    } else {
      const error = res.errors?.[0] || 'Unable to mark complaint as irrelevant.';
      setIrrelevantError(error);
      toast.error(error);
    }
  }

  if (loading) return <LoadingSpinner />;
  if (!complaint) return <div className="empty-state"><strong>Complaint not found</strong></div>;

  const currentStatus = normalizeStatus(complaint.status);
  const isLocked = complaint.isDuplicate || currentStatus === ComplaintStatus.Resolved || currentStatus === ComplaintStatus.Irrelevant;
  const allowedStatuses = getAllowedStatuses(currentStatus);
  const activityCount = complaint.updates?.length || 0;

  return (
    <div className="complaint-detail-page">
      <Link to={ROUTES.ADMIN_COMPLAINTS} className="back-link">Back to All Complaints</Link>

      <section className="detail-card detail-hero complaint-details-card" aria-labelledby="complaint-details-title">
        <div className="detail-heading">
          <div>
            <span className="eyebrow">Complaint #{String(complaint.id).padStart(3, '0')}</span>
            <h1 id="complaint-details-title">Complaint Details</h1>
          </div>
          {!isLocked && (
            <button type="button" className="btn-outline btn-irrelevant-action" onClick={openIrrelevantDialog} disabled={updating}>
              Mark Irrelevant
            </button>
          )}
        </div>

        <dl className="complaint-readonly-grid">
          <div className="complaint-readonly-item">
            <dt>Complaint ID</dt>
            <dd>#{String(complaint.id).padStart(3, '0')}</dd>
          </div>
          <div className="complaint-readonly-item complaint-readonly-title">
            <dt>Title</dt>
            <dd>{complaint.title}</dd>
          </div>
          <div className="complaint-readonly-item">
            <dt>Category</dt>
            <dd><CategoryBadge category={complaint.category} /></dd>
          </div>
          <div className="complaint-readonly-item">
            <dt>Current Status</dt>
            <dd><StatusBadge status={complaint.status} /></dd>
          </div>
          <div className="complaint-readonly-item complaint-readonly-description">
            <dt>Description</dt>
            <dd>{complaint.description}</dd>
          </div>
        </dl>
      </section>

      <section className="detail-card complaint-summary-card" aria-labelledby="complaint-summary-title">
        <h2 id="complaint-summary-title">Complaint Summary</h2>
        <dl className="complaint-summary-grid">
          <div><dt>Current Status</dt><dd>{formatStatus(complaint.status)}</dd></div>
          <div><dt>Category</dt><dd>{complaint.category}</dd></div>
          <div><dt>Created</dt><dd>{formatDateTime(complaint.createdAt)}</dd></div>
          <div><dt>Last Updated</dt><dd>{complaint.updatedAt ? formatDateTime(complaint.updatedAt) : 'Not yet updated'}</dd></div>
          <div><dt>Activity Count</dt><dd>{activityCount}</dd></div>
        </dl>
      </section>

      <section className="detail-card complaint-update-card" aria-labelledby="complaint-update-title">
        <div className="complaint-update-heading">
          <div>
            <h2 id="complaint-update-title">Update Complaint</h2>
            <p>Review changes, then submit once.</p>
          </div>
        </div>

        {isLocked && (
          <div className="complaint-finalized-message" role="status">
            <strong>Complaint Closed</strong>
            <span>This complaint has been finalized and cannot be modified.</span>
          </div>
        )}

        <form className="complaint-update-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="complaint-status">Status</label>
            <select
              id="complaint-status"
              value={form.status}
              disabled={isLocked || updating || markingIrrelevant}
              onChange={(event) => updateForm('status', Number(event.target.value))}
            >
              {allowedStatuses.map((status) => (
                <option key={status} value={status}>
                  {status === currentStatus ? `${formatStatus(status)} (Current)` : formatStatus(status)}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="resolution-note">Resolution Note</label>
            <textarea
              id="resolution-note"
              value={form.note}
              disabled={isLocked || updating || markingIrrelevant}
              maxLength={2000}
              placeholder="Add an internal resolution note or progress update"
              onChange={(event) => updateForm('note', event.target.value)}
            />
          </div>

          <label className="update-checkbox">
            <input
              type="checkbox"
              checked={form.isDuplicate}
              disabled={isLocked || updating || markingIrrelevant}
              onChange={(event) => updateForm('isDuplicate', event.target.checked)}
            />
            <span>Mark as Duplicate</span>
          </label>

          {formError && <p className="complaint-update-error" role="alert">{formError}</p>}

          <div className="complaint-update-footer">
            <button type="submit" className="btn-submit" disabled={isLocked || updating || markingIrrelevant}>
              {updating ? 'Updating...' : 'Update Complaint'}
            </button>
          </div>
        </form>
      </section>

      <section className="detail-card complaint-timeline-card" aria-labelledby="activity-timeline-title">
        <div className="timeline-heading">
          <div>
            <h2 id="activity-timeline-title">Activity Timeline</h2>
            <p>{activityCount} recorded {activityCount === 1 ? 'activity' : 'activities'}</p>
          </div>
        </div>
        {activityCount === 0 ? (
          <p className="muted-text">No activity has been recorded yet.</p>
        ) : (
          <div className="timeline">
            {complaint.updates.map((update, index) => {
              const activity = describeActivity(update);
              return (
                <article className="timeline-item" key={update.id || index}>
                  <div className="timeline-item-header">
                    <span className="timeline-item-type">{activity.title}</span>
                    <time className="timeline-item-date">{formatDateTime(update.createdAt)}</time>
                  </div>
                  <div className="timeline-item-body">{activity.detail}</div>
                  {activity.actor && <div className="timeline-item-actor">{activity.actor}</div>}
                </article>
              );
            })}
          </div>
        )}
      </section>

      {irrelevantDialogOpen && (
        <div className="complaint-modal-backdrop" role="presentation">
          <form className="complaint-irrelevant-dialog" onSubmit={handleMarkIrrelevant} role="dialog" aria-modal="true" aria-labelledby="irrelevant-dialog-title">
            <div className="complaint-dialog-heading">
              <h2 id="irrelevant-dialog-title">Mark Complaint Irrelevant</h2>
              <button type="button" className="dialog-close" onClick={() => setIrrelevantDialogOpen(false)} disabled={markingIrrelevant} aria-label="Close dialog">x</button>
            </div>
            <p>Provide the reason before finalizing this complaint. This action cannot be undone.</p>
            <div className="form-group">
              <label htmlFor="irrelevant-reason">Reason</label>
              <textarea
                id="irrelevant-reason"
                autoFocus
                value={irrelevantReason}
                disabled={markingIrrelevant}
                maxLength={2000}
                placeholder="Explain why this complaint is irrelevant"
                onChange={(event) => {
                  setIrrelevantReason(event.target.value);
                  if (irrelevantError) setIrrelevantError('');
                }}
              />
            </div>
            {irrelevantError && <p className="complaint-update-error" role="alert">{irrelevantError}</p>}
            <div className="complaint-dialog-footer">
              <button type="button" className="btn-cancel" onClick={() => setIrrelevantDialogOpen(false)} disabled={markingIrrelevant}>Cancel</button>
              <button type="submit" className="btn-danger" disabled={markingIrrelevant}>{markingIrrelevant ? 'Finalizing...' : 'Mark Irrelevant'}</button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
}
