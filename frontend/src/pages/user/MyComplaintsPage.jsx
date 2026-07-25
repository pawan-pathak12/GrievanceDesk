import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { complaintApi } from '../../api/complaintApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';
import { formatDate, formatTime } from '../../utils/enums';
import { markItemsViewed } from '../../utils/viewTracking';
import { Eye } from 'lucide-react';
import TableActionButton from '../../components/common/TableActionButton';

export default function MyComplaintsPage() {
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const toast = useToast();
  const { user } = useAuth();
  const userKey = user?.userId || user?.id || user?.email || 'member';

  useEffect(() => {
    loadComplaints();
  }, []);

  async function loadComplaints() {
    setLoading(true);
    const res = await complaintApi.getMy();
    if (res.isSuccess) {
      const items = res.data || [];
      setComplaints(items);
      markItemsViewed(userKey, 'complaints', items);
    }
    else toast.error(res.errors?.[0] || 'Failed to load complaints');
    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header page-header-row">
        <div>
          <h1>My Complaints</h1>
          <p>Review every complaint you have submitted.</p>
        </div>
        <button className="btn-submit" onClick={() => navigate(ROUTES.NEW_COMPLAINT)}>
          New Complaint
        </button>
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <div>
            <h2>All Submissions</h2>
            <span>Showing {complaints.length} complaints</span>
          </div>
        </div>
        {complaints.length === 0 ? (
          <div className="empty-state">
            <strong>No complaints submitted yet</strong>
            <p>When you submit one, its status and history will be available here.</p>
          </div>
        ) : (
          <div className="table-scroll">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Case</th>
                  <th>Category</th>
                  <th>Subject</th>
                  <th>Status</th>
                  <th>Date</th>
                  <th>Time</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {complaints.map((c) => (
                  <tr key={c.id}>
                    <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                    <td><CategoryBadge category={c.category} /></td>
                    <td className="table-title">
                      {c.title}
                      {c.isEdited && <span className="inline-flag">Edited</span>}
                    </td>
                    <td><StatusBadge status={c.status} /></td>
                    <td>{formatDate(c.createdAt)}</td>
                    <td>{formatTime(c.createdAt)}</td>
                    <td>
                      <TableActionButton label="View complaint" icon={Eye} onClick={() => navigate(ROUTES.COMPLAINT_DETAIL.replace(':id', c.id))} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
