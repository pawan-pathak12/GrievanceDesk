import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import ComplaintDiscussionModal from '../../components/common/ComplaintDiscussionModal';
import { useToast } from '../../context/ToastContext';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';
import { CategoryOptions, StatusOptions, formatDate, formatTime } from '../../utils/enums';
import { markItemsViewed } from '../../utils/viewTracking';
import { Eye, MessageSquare } from 'lucide-react';
import TableActionButton from '../../components/common/TableActionButton';

export default function AdminComplaintsPage() {
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState({ status: '', category: '', search: '' });
  const [discussionComplaint, setDiscussionComplaint] = useState(null);
  const navigate = useNavigate();
  const toast = useToast();
  const { user } = useAuth();
  const userKey = user?.userId || user?.id || user?.email || 'member';

  const loadComplaints = useCallback(async (activeFilters) => {
    setLoading(true);
    const params = {};
    if (activeFilters.status !== '') params.status = Number(activeFilters.status);
    if (activeFilters.category !== '') params.category = Number(activeFilters.category);
    if (activeFilters.search.trim()) params.search = activeFilters.search.trim();

    const res = await adminApi.getAll(params);
    if (res.isSuccess) {
      const items = res.data || [];
      setComplaints(items);
      markItemsViewed(userKey, 'adminComplaints', items);
    }
    else toast.error(res.errors?.[0] || 'Failed to load complaints');
    setLoading(false);
  }, [toast, userKey]);

  useEffect(() => {
    const delay = filters.search.trim() ? 300 : 0;
    const timeout = window.setTimeout(() => loadComplaints(filters), delay);

    return () => window.clearTimeout(timeout);
  }, [filters, loadComplaints]);

  return (
    <div>
      <div className="page-header">
        <h1>All Complaints</h1>
        <p>Filter, review, and action every complaint in the system.</p>
      </div>

      <div className="filter-bar">
        <div className="filter-field">
          <label>Status</label>
          <select value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value })}>
            <option value="">All statuses</option>
            {StatusOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
          </select>
        </div>
        <div className="filter-field">
          <label>Category</label>
          <select value={filters.category} onChange={(e) => setFilters({ ...filters, category: e.target.value })}>
            <option value="">All categories</option>
            {CategoryOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
          </select>
        </div>
        <div className="filter-field filter-search">
          <label>Search</label>
          <input
            type="text"
            placeholder="Search by title"
            value={filters.search}
            onChange={(e) => setFilters({ ...filters, search: e.target.value })}
          />
        </div>
      </div>

      {loading ? <LoadingSpinner /> : (
        <div className="data-table-wrapper">
          <div className="table-header">
            <div>
              <h2>Complaints</h2>
              <span>Showing {complaints.length} complaints</span>
            </div>
          </div>
          {complaints.length === 0 ? (
            <div className="empty-state"><strong>No complaints found</strong></div>
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
                  <th>Time</th>
                  <th></th>
                  </tr>
                </thead>
                <tbody>
                  {complaints.map((c) => (
                    <tr key={c.id}>
                      <td><span className="table-id">#{String(c.id).padStart(3, '0')}</span></td>
                      <td className="table-title">{c.title}</td>
                      <td><CategoryBadge category={c.category} /></td>
                      <td><StatusBadge status={c.status} /></td>
                      <td>{formatDate(c.createdAt)}</td>
                      <td>{formatTime(c.createdAt)}</td>
                      <td>
                        <div className="table-icon-actions">
                          <TableActionButton label="View complaint" icon={Eye} onClick={() => navigate(ROUTES.ADMIN_COMPLAINT_DETAIL.replace(':id', c.id))} />
                          <TableActionButton label="Open discussion" icon={MessageSquare} onClick={() => setDiscussionComplaint(c)} />
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {discussionComplaint && <ComplaintDiscussionModal complaint={discussionComplaint} isAdmin onClose={() => setDiscussionComplaint(null)} />}
    </div>
  );
}
