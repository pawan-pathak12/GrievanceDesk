import { useEffect, useState } from 'react';
import { MessageSquare } from 'lucide-react';
import { complaintApi } from '../../api/complaintApi';
import StatusBadge from '../../components/common/StatusBadge';
import CategoryBadge from '../../components/common/CategoryBadge';
import ComplaintDiscussionModal from '../../components/common/ComplaintDiscussionModal';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import TableActionButton from '../../components/common/TableActionButton';
import { useToast } from '../../context/ToastContext';
import { formatDate, formatTime } from '../../utils/enums';

export default function AllComplaintsPage() {
  const [complaints, setComplaints] = useState([]);
  const [loading, setLoading] = useState(true);
  const [discussionComplaint, setDiscussionComplaint] = useState(null);
  const toast = useToast();

  useEffect(() => {
    loadComplaints();
  }, []);

  async function loadComplaints() {
    setLoading(true);
    const result = await complaintApi.getAll();
    if (result.isSuccess) {
      setComplaints(result.data || []);
    } else {
      toast.error(result.errors?.[0] || 'Failed to load complaints.');
    }
    setLoading(false);
  }

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="page-header">
        <h1>All Complaints</h1>
        <p>Browse complaint activity shared across your organization.</p>
      </div>

      <div className="data-table-wrapper">
        <div className="table-header">
          <div>
            <h2>Organization Complaints</h2>
            <span>Showing {complaints.length} complaints</span>
          </div>
        </div>
        {complaints.length === 0 ? (
          <div className="empty-state"><strong>No complaints available.</strong></div>
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
                {complaints.map((complaint) => (
                  <tr key={complaint.id}>
                    <td><span className="table-id">#{String(complaint.id).padStart(3, '0')}</span></td>
                    <td><CategoryBadge category={complaint.category} /></td>
                    <td className="table-title">{complaint.title}</td>
                    <td><StatusBadge status={complaint.status} /></td>
                    <td>{formatDate(complaint.createdAt)}</td>
                    <td>{formatTime(complaint.createdAt)}</td>
                    <td>
                      <TableActionButton label="Open discussion" icon={MessageSquare} onClick={() => setDiscussionComplaint(complaint)} />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {discussionComplaint && <ComplaintDiscussionModal complaint={discussionComplaint} onClose={() => setDiscussionComplaint(null)} />}
    </div>
  );
}
