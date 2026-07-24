import { StatusLabels } from '../../utils/enums';

const statusColors = {
  0: 'pending',
  1: 'inprogress',
  2: 'resolved',
  3: 'irrelevant',
};

export default function StatusBadge({ status }) {
  const cls = statusColors[status] || 'pending';
  const label = StatusLabels[status] || 'Unknown';
  return <span className={`badge badge-status badge-${cls}`}>{label}</span>;
}
