import { X } from 'lucide-react';
import StatusBadge from './StatusBadge';
import ComplaintDiscussion from './ComplaintDiscussion';

export default function ComplaintDiscussionModal({ complaint, onClose, isAdmin = false }) {
  return (
    <div className="complaint-modal-backdrop discussion-modal-backdrop" role="presentation" onMouseDown={(event) => event.target === event.currentTarget && onClose()}>
      <section className="complaint-discussion-dialog" role="dialog" aria-modal="true" aria-labelledby="discussion-modal-title">
        <header className="discussion-modal-header">
          <div>
            <h2 id="discussion-modal-title">Complaint Discussion</h2>
            <p>{complaint.title}</p>
            <StatusBadge status={complaint.status} />
          </div>
          <button type="button" className="dialog-close" onClick={onClose} aria-label="Close discussion"><X size={18} /></button>
        </header>

        <div className="discussion-modal-body">
          <ComplaintDiscussion complaintId={complaint.id} isAdmin={isAdmin} embedded autoFocus autoScrollOnLoad />
        </div>

        <footer className="discussion-modal-footer">
          <button type="button" className="btn-cancel" onClick={onClose}>Close</button>
        </footer>
      </section>
    </div>
  );
}
