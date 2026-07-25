export default function NoticePriorityBadge({ priority = 'Normal' }) {
  const normalized = String(priority).toLowerCase();

  return <span className={`notice-priority-badge notice-priority-${normalized}`}>{priority}</span>;
}
