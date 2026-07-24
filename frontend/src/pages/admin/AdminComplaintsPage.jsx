import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import { ComplaintStatus, ComplaintCategory } from '../../utils/enums';

export default function AdminComplaintsPage() {
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState({ status: '', category: '', search: '' });
  const navigate = useNavigate();
  const toast = useToast();

  useEffect(() => {
    loadComplaints();
  }, []);

  async function loadComplaints() {
    setLoading(true);
    const params = {};
    if (filters.status !== '') params.status = parseInt(filters.status);
    if (filters.category !== '') params.category = parseInt(filters.category);
    if (filters.search) params.search = filters.search;

    const res = await adminApi.getAll(params);
    if (res.isSuccess) setComplaints(res.data || []);
    else toast.error(res.errors?.[0] || 'Failed to load complaints');
    setLoading(false);
  }

  function handleFilter() {
    loadComplaints();
  }

  return (
    <div>
      <div className="page-header">
        <h1>Admin — All Complaints</h1>
        <p>Manage all system complaints</p>
      </div>

      {/* Filters */}
      <div style={{ display: 'flex', gap: 12, marginBottom: 20, flexWrap: 'wrap', alignItems: 'flex-end' }}>
        <div>
          <label style={{ fontSize: '0.8rem', color: 'var(--text-gray)', display: 'block', marginBottom: 4 }}>Status</label>
          <select className="status-select" value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value })}>
            <option value="">All Statuses</option>
            {Object.entries(ComplaintStatus).map(([label, val]) => (
              <option key={val} value={val}>{label}</option>
            ))}
          </select>
        </div>
        <div>
          <label style={{ fontSize: '0.8rem', color: 'var(--text-gray)', display: 'block', marginBottom: 4 }}>Category</label>
          <select className="status-select" value={filters.category} onChange={(e) => setFilters({ ...filters, category: e.target.value })}>
            <option value="">All Categories</option>
            {Object.entries(ComplaintCategory).map(([label, val]) => (
              <option key={val} value={val}>{label}</option>
            ))}
          </select>
        </div>
        <div>
          <label style={{ fontSize: '0.8rem', color: 'var(--text-gray)', display: 'block', marginBottom: 4 }}>Search</label>
          <input
            type="text"
            className="status-select"
            placeholder="Search by title..."
            value={filters.search}
            onChange={(e) => setFilters({ ...filters, search: e.target.value })}
            style={{ width: 200 }}
          />
        </div>
        <button className="btn-outline" onClick={handleFilter}>🔍 Filter</button>
      </div>

      {loading ? <LoadingSpinner /> : (
        <div className="data-table-wrapper">
          <div className="table-header">
            <h2>Complaints</h2>
            <span style={{ fontSize: '0.8rem', color: 'var(--text-gray)' }}>
              Showing {complaints.length} complaints
            </span>
          </div>
          {complaints.length === 0 ? (
            <div className="empty-state"><p>No complaints found.</p></div>
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
                {complaints.map((c) => (
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
      )}
    </div>
  );
}
