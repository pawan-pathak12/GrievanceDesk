import '../../styles/auth.css';

export default function AuthBrand() {
  return (
    <div className="auth-brand">
      <div className="auth-brand-content">
        <div className="auth-brand-icon">🎓</div>
        <h1>GrievanceDesk</h1>
        <p>Institutional Efficiency & Fairness — A centralized platform for transparent complaint management</p>

        <div className="auth-brand-features">
          <div className="auth-brand-feature">
            <div className="feature-icon">🔒</div>
            <span>Organization-controlled access with secure authentication</span>
          </div>
          <div className="auth-brand-feature">
            <div className="feature-icon">📊</div>
            <span>Real-time dashboard with analytics & tracking</span>
          </div>
          <div className="auth-brand-feature">
            <div className="feature-icon">🔄</div>
            <span>Full complaint lifecycle with audit trail</span>
          </div>
        </div>
      </div>
    </div>
  );
}
