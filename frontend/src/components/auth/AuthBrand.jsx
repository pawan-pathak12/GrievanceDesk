import '../../Styles/auth.css';

export default function AuthBrand() {
  return (
    <div className="auth-brand">
      <div className="auth-brand-content">
        <div className="auth-brand-icon">GD</div>
        <h1>GrievanceDesk</h1>
        <p>A secure complaint management workspace for transparent campus operations.</p>

        <div className="auth-brand-features">
          <div className="auth-brand-feature">
            <div className="feature-icon">01</div>
            <span>Verified access for organization members</span>
          </div>
          <div className="auth-brand-feature">
            <div className="feature-icon">02</div>
            <span>Clear status tracking from submission to resolution</span>
          </div>
          <div className="auth-brand-feature">
            <div className="feature-icon">03</div>
            <span>Admin review tools with a complete activity trail</span>
          </div>
        </div>
      </div>
    </div>
  );
}
