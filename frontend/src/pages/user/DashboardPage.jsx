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

    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  const recent = complaints.slice(0, 5);

  return (
    <div>
      <div className="page-header">
        <h1>My Submissions</h1>
        <p>Tracking your {stats?.totalComplaints || 0} active and past institutional complaints.</p>
      </div>

      <div className="stats-grid">
        <StatCard label="Total Complaints" value={stats?.totalComplaints || 0} icon="📋" />
        <StatCard label="Resolved" value={stats?.resolved || 0} icon="✅" />
        <StatCard label="In Progress" value={stats?.inProgress || 0} icon="🔄" />
        <StatCard label="Pending" value={stats?.pending || 0} icon="💬" />
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <h2>Recent Submissions</h2>
        </div>
        {recent.length === 0 ? (
          <div className="empty-state">
            <p>No complaints submitted yet.</p>
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
              {recent.map((c) => (
                <tr key={c.id}>
                  <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                  <td><CategoryBadge category={c.category} /></td>
                  <td>{c.title}</td>
                  <td><StatusBadge status={c.status} /></td>
                  <td>{new Date(c.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</td>
                  <td>
                    <span className="table-link" onClick={() => navigate(ROUTES.COMPLAINT_DETAIL.replace(':id', c.id))}>
                      👁️
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
