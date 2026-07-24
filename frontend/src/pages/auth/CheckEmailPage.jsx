import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import '../../styles/auth.css';

export default function CheckEmailPage() {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  async function handleSubmit(e) {
    e.preventDefault();
    if (!email) return;

    setLoading(true);
    const res = await authApi.checkEmail(email);
    setLoading(false);

    if (res.isSuccess) {
      toast.success(res.data?.message || 'OTP sent to your email');
      navigate(ROUTES.REGISTER_OTP, { state: { email } });
    } else {
      toast.error(res.errors?.[0] || 'Email verification failed');
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-logo">
        <div className="auth-logo-icon">🎓</div>
        <h1>GrievanceDesk</h1>
        <p>Institutional Efficiency & Fairness</p>
      </div>

      <div className="auth-card">
        <h2>Register</h2>
        <p className="subtitle">Enter your organization email to get started</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Email</label>
            <div className="input-wrapper">
              <span className="input-icon">✉️</span>
              <input
                type="email"
                placeholder="your-email@organization.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Sending OTP...' : 'Send Verification Code →'}
          </button>
        </form>

        <div className="auth-divider">or</div>

        <div className="auth-footer">
          Already have an account? <Link to={ROUTES.LOGIN}>Back to Login</Link>
        </div>
      </div>
    </div>
  );
}
