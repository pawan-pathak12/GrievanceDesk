import { useState, useEffect } from 'react';
import { dashboardApi } from '../../api/dashboardApi';
import { adminApi } from '../../api/adminApi';
import { useNavigate } from 'react-router-dom';
import StatCard from '../../components/common/StatCard';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';

export default function AdminDashboardPage() {
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
    const [statsRes, allRes] = await Promise.all([
      dashboardApi.getAdmin(),
      adminApi.getAll(),
    ]);

    if (statsRes.isSuccess) setStats(statsRes.data);
    else toast.error(statsRes.errors?.[0] || 'Failed to load dashboard');

    if (allRes.isSuccess) setComplaints(allRes.data || []);

    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  const recent = complaints.slice(0, 5);

  return (
    <div>
      <div className="page-header">
        <h1>Admin Dashboard</h1>
        <p>System-wide complaint overview</p>
      </div>

      <div className="stats-grid">
        <StatCard label="Total Complaints" value={stats?.totalComplaints || 0} icon="📋" />
        <StatCard label="Resolved" value={stats?.resolved || 0} icon="✅" />
        <StatCard label="In Progress" value={stats?.inProgress || 0} icon="🔄" />
        <StatCard label="Pending" value={stats?.pending || 0} icon="💬" />
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <h2>Recent Complaints</h2>
        </div>
        {recent.length === 0 ? (
          <div className="empty-state"><p>No complaints in the system.</p></div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Subject</th>
                <th>Category</th>
                <th>Status</th>
                <th>Date</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {recent.map((c) => (
                <tr key={c.id}>
                  <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                  <td>{c.title}</td>
                  <td><CategoryBadge category={c.category} /></td>
                  <td><StatusBadge status={c.status} /></td>
                  <td>{new Date(c.createdAt).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</td>
                  <td>
                    <span className="table-link" onClick={() => navigate(ROUTES.ADMIN_COMPLAINT_DETAIL.replace(':id', c.id))}>
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
