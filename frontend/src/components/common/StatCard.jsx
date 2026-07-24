import '../../styles/components.css';

export default function StatCard({ label, value, icon, color }) {
  return (
    <div className={`stat-card stat-card-${color || 'default'}`}>
      <div className="stat-card-info">
        <span className="stat-card-label">{label}</span>
        <span className="stat-card-value">{String(value).padStart(2, '0')}</span>
      </div>
      <div className="stat-card-icon">{icon}</div>
    </div>
  );
}
