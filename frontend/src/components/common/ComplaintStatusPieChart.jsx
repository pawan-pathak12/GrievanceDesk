import { useMemo, useState } from 'react';

const STATUS_CONFIG = [
  { key: 'Pending', label: 'Pending', color: '#d97706' },
  { key: 'InProgress', label: 'In Progress', color: '#2563eb' },
  { key: 'Resolved', label: 'Resolved', color: '#059669' },
  { key: 'Completed', label: 'Completed', color: '#7c3aed' },
  { key: 'Irrelevant', label: 'Irrelevant', color: '#64748b' },
];

const NUMERIC_STATUSES = {
  0: 'Pending',
  1: 'InProgress',
  2: 'Resolved',
  3: 'Irrelevant',
  4: 'Completed',
};

function statusKey(status) {
  if (typeof status === 'number') return NUMERIC_STATUSES[status];
  if (typeof status !== 'string') return undefined;

  const compact = status.replace(/[\s_-]/g, '').toLowerCase();
  return {
    pending: 'Pending',
    inprogress: 'InProgress',
    resolved: 'Resolved',
    completed: 'Completed',
    irrelevant: 'Irrelevant',
  }[compact];
}

function pointAt(angle, radius) {
  const radians = ((angle - 90) * Math.PI) / 180;
  return {
    x: 120 + radius * Math.cos(radians),
    y: 120 + radius * Math.sin(radians),
  };
}

function arcPath(startAngle, endAngle) {
  const start = pointAt(endAngle, 88);
  const end = pointAt(startAngle, 88);
  const largeArc = endAngle - startAngle > 180 ? 1 : 0;

  return `M 120 120 L ${start.x} ${start.y} A 88 88 0 ${largeArc} 0 ${end.x} ${end.y} Z`;
}

export default function ComplaintStatusPieChart({ complaints }) {
  const [activeKey, setActiveKey] = useState(null);

  const { total, data } = useMemo(() => {
    const counts = Object.fromEntries(STATUS_CONFIG.map(({ key }) => [key, 0]));

    complaints.forEach((complaint) => {
      const key = statusKey(complaint.status);
      if (key) counts[key] += 1;
    });

    const totalComplaints = complaints.length;
    return {
      total: totalComplaints,
      data: STATUS_CONFIG.map((status) => ({
        ...status,
        count: counts[status.key],
        percentage: totalComplaints ? Number(((counts[status.key] / totalComplaints) * 100).toFixed(1)) : 0,
      })),
    };
  }, [complaints]);

  if (total === 0) {
    return (
      <section className="status-distribution-card" aria-labelledby="status-distribution-title">
        <div className="status-distribution-header">
          <div>
            <h2 id="status-distribution-title">Complaint Status Distribution</h2>
            <p>Current complaint volume by status.</p>
          </div>
        </div>
        <div className="status-distribution-empty">
          <strong>No complaints to visualize yet</strong>
          <span>Complaint status distribution will appear here once records are available.</span>
        </div>
      </section>
    );
  }

  let currentAngle = 0;
  const slices = data
    .filter((status) => status.count > 0)
    .map((status) => {
      const startAngle = currentAngle;
      const endAngle = currentAngle + (status.percentage / 100) * 360;
      currentAngle = endAngle;
      return { ...status, startAngle, endAngle };
    });

  const activeStatus = data.find((status) => status.key === activeKey);

  return (
    <section className="status-distribution-card" aria-labelledby="status-distribution-title">
      <div className="status-distribution-header">
        <div>
          <h2 id="status-distribution-title">Complaint Status Distribution</h2>
          <p>{total} total complaint{total === 1 ? '' : 's'}</p>
        </div>
      </div>

      <div className="status-distribution-content">
        <div className="status-chart-visual">
          <svg className="status-chart-svg" viewBox="0 0 240 240" role="img" aria-label="Complaint status distribution pie chart">
            <circle cx="120" cy="120" r="88" fill="#f1f5f9" />
            {slices.map((status) => (
              status.percentage === 100 ? (
                <circle
                  key={status.key}
                  className="status-chart-slice"
                  cx="120"
                  cy="120"
                  r="88"
                  fill={status.color}
                  tabIndex="0"
                  aria-label={`${status.label}: ${status.count} complaints, ${status.percentage}%`}
                  onMouseEnter={() => setActiveKey(status.key)}
                  onMouseLeave={() => setActiveKey(null)}
                  onFocus={() => setActiveKey(status.key)}
                  onBlur={() => setActiveKey(null)}
                />
              ) : (
                <path
                  key={status.key}
                  className="status-chart-slice"
                  d={arcPath(status.startAngle, status.endAngle)}
                  fill={status.color}
                  tabIndex="0"
                  aria-label={`${status.label}: ${status.count} complaints, ${status.percentage}%`}
                  onMouseEnter={() => setActiveKey(status.key)}
                  onMouseLeave={() => setActiveKey(null)}
                  onFocus={() => setActiveKey(status.key)}
                  onBlur={() => setActiveKey(null)}
                />
              )
            ))}
            <circle cx="120" cy="120" r="52" fill="#ffffff" />
            <text x="120" y="116" textAnchor="middle" className="status-chart-total">{total}</text>
            <text x="120" y="136" textAnchor="middle" className="status-chart-caption">Total</text>
          </svg>

          {activeStatus && (
            <div className="status-chart-tooltip" role="status">
              <strong>{activeStatus.label}</strong>
              <span>{activeStatus.count} complaint{activeStatus.count === 1 ? '' : 's'} ({activeStatus.percentage}%)</span>
            </div>
          )}
        </div>

        <ul className="status-distribution-legend" aria-label="Complaint status legend">
          {data.map((status) => (
            <li key={status.key}>
              <span className="status-legend-swatch" style={{ backgroundColor: status.color }} />
              <span className="status-legend-label">{status.label}</span>
              <span className="status-legend-count">{status.count}</span>
              <span className="status-legend-percentage">{status.percentage}%</span>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}
