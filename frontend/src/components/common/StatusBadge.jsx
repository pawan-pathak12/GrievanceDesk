import { formatStatus, normalizeStatus } from '../../utils/enums';

const statusColors = {
  0: 'pending',
  1: 'inprogress',
  2: 'resolved',
  3: 'irrelevant',
};

export default function StatusBadge({ status }) {
  const normalized = normalizeStatus(status);
  const cls = statusColors[normalized] || 'pending';
  const label = formatStatus(status);
  return <span className={`badge badge-status badge-${cls}`}>{label}</span>;
}
