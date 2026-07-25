import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { dashboardApi } from '../../api/dashboardApi';
import { complaintApi } from '../../api/complaintApi';
import StatCard from '../../components/common/StatCard';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { formatDate, formatTime } from '../../utils/enums';
import { Eye } from 'lucide-react';
import TableActionButton from '../../components/common/TableActionButton';

export default function DashboardPage() {
  const [stats, setStats] = useState(null);
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const toast = useToast();

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    setLoading(true);
    const [statsRes, complaintsRes] = await Promise.all([
      dashboardApi.getUser(),
      complaintApi.getMy(),
    ]);

    if (statsRes.isSuccess) setStats(statsRes.data);
    else toast.error(statsRes.errors?.[0] || 'Failed to load dashboard');

    if (complaintsRes.isSuccess) setComplaints(complaintsRes.data || []);
    else toast.error(complaintsRes.errors?.[0] || 'Failed to load complaints');

    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  const recent = complaints.slice(0, 5);

  return (
    <div>
      <div className="page-header page-header-row">
        <div>
          <h1>My Submissions</h1>
          <p>Track active and resolved complaints across your institution.</p>
        </div>
        <button className="btn-submit" onClick={() => navigate(ROUTES.NEW_COMPLAINT)}>
          New Complaint
        </button>
      </div>

      <div className="stats-grid">
        <StatCard label="Total Complaints" value={stats?.totalComplaints || 0} icon="T" />
        <StatCard label="Resolved" value={stats?.resolved || 0} icon="R" color="success" />
        <StatCard label="In Progress" value={stats?.inProgress || 0} icon="P" color="info" />
        <StatCard label="Pending" value={stats?.pending || 0} icon="N" color="warning" />
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <div>
            <h2>Recent Submissions</h2>
            <span>{recent.length} latest complaints</span>
          </div>
          <button className="btn-outline" onClick={() => navigate(ROUTES.MY_COMPLAINTS)}>
            View All
          </button>
        </div>
        {recent.length === 0 ? (
          <div className="empty-state">
            <strong>No complaints submitted yet</strong>
            <p>Create your first complaint and it will appear here.</p>
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
                {recent.map((c) => (
                  <tr key={c.id}>
                    <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                    <td><CategoryBadge category={c.category} /></td>
                    <td className="table-title">{c.title}</td>
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
