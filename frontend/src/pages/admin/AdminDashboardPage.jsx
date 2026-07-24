import { useState, useEffect } from 'react';
import { dashboardApi } from '../../api/dashboardApi';
import { adminApi } from '../../api/adminApi';
import { useNavigate } from 'react-router-dom';
import StatCard from '../../components/common/StatCard';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import ComplaintStatusPieChart from '../../components/common/ComplaintStatusPieChart';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { formatDate } from '../../utils/enums';

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
    else toast.error(allRes.errors?.[0] || 'Failed to load complaints');

    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  const recent = complaints.slice(0, 5);

  return (
    <div>
      <div className="page-header">
        <h1>Admin Dashboard</h1>
        <p>Monitor complaint volume, resolution progress, and recent activity.</p>
      </div>

      <div className="stats-grid">
        <StatCard label="Total" value={stats?.totalComplaints || 0} icon="T" />
        <StatCard label="Today" value={stats?.todayComplaints || 0} icon="D" color="info" />
        <StatCard label="Resolved" value={stats?.resolved || 0} icon="R" color="success" />
        <StatCard label="Pending" value={stats?.pending || 0} icon="P" color="warning" />
      </div>

      <ComplaintStatusPieChart complaints={complaints} />

      <div className="data-table-wrapper">
        <div className="table-header">
          <div>
            <h2>Recent Complaints</h2>
            <span>{recent.length} newest complaints</span>
          </div>
          <button className="btn-outline" onClick={() => navigate(ROUTES.ADMIN_COMPLAINTS)}>
            Manage All
          </button>
        </div>
        {recent.length === 0 ? (
          <div className="empty-state"><strong>No complaints in the system</strong></div>
        ) : (
          <div className="table-scroll">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Case</th>
                  <th>Subject</th>
                  <th>Category</th>
                  <th>Status</th>
                  <th>Date</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                {recent.map((c) => (
                  <tr key={c.id}>
                    <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                    <td className="table-title">{c.title}</td>
                    <td><CategoryBadge category={c.category} /></td>
                    <td><StatusBadge status={c.status} /></td>
                    <td>{formatDate(c.createdAt)}</td>
                    <td>
                      <button className="table-link" onClick={() => navigate(ROUTES.ADMIN_COMPLAINT_DETAIL.replace(':id', c.id))}>
                        Open
                      </button>
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
