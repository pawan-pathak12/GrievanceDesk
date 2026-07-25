import '../../Styles/auth.css';

export default function AuthBrand() {
  return (
    <div className="auth-brand">
      <span className="auth-brand-circle auth-brand-circle-top" aria-hidden="true" />
      <span className="auth-brand-circle auth-brand-circle-bottom" aria-hidden="true" />
      <div className="auth-brand-content">
        <div className="auth-brand-icon">GD</div>
        <h1>GrievanceDesk</h1>

        <div className="auth-brand-features">
          <div className="auth-brand-feature">
            <span className="feature-icon" aria-hidden="true">✓</span>
            <span>Secure Complaint Submission</span>
          </div>
          <div className="auth-brand-feature">
            <span className="feature-icon" aria-hidden="true">✓</span>
            <span>Complaint Status Tracking</span>
          </div>
          <div className="auth-brand-feature">
            <span className="feature-icon" aria-hidden="true">✓</span>
            <span>Admin Resolution Workflow</span>
          </div>
        </div>
      </div>
      <div className="auth-brand-version">GrievanceDesk • v1.0</div>
    </div>
  );
}
