import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { complaintApi } from '../../api/complaintApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';

export default function MyComplaintsPage() {
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const toast = useToast();

  useEffect(() => {
    loadComplaints();
  }, []);

  async function loadComplaints() {
    setLoading(true);
    const res = await complaintApi.getMy();
    if (res.isSuccess) setComplaints(res.data || []);
    else toast.error(res.errors?.[0] || 'Failed to load complaints');
    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1>My Complaints</h1>
          <p>All your submitted complaints</p>
        </div>
        <button className="btn-submit" onClick={() => navigate(ROUTES.NEW_COMPLAINT)}>
          + New Complaint
        </button>
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <h2>All Submissions</h2>
          <span style={{ fontSize: '0.8rem', color: 'var(--text-gray)' }}>
            Showing {complaints.length} complaints
          </span>
        </div>
        {complaints.length === 0 ? (
          <div className="empty-state">
            <p>You haven't submitted any complaints yet.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Case ID</th>
                <th>Category</th>
                <th>Subject</th>
                <th>Status</th>
                <th>Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {complaints.map((c) => (
                <tr key={c.id}>
                  <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                  <td><CategoryBadge category={c.category} /></td>
                  <td>{c.title} {c.isEdited && <span style={{ fontSize: '0.7rem', color: 'var(--warning)' }}>edited</span>}</td>
                  <td><StatusBadge status={c.status} /></td>
                  <td>{new Date(c.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</td>
                  <td>
                    <span className="table-link" onClick={() => navigate(ROUTES.COMPLAINT_DETAIL.replace(':id', c.id))}>
                      View Details
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
