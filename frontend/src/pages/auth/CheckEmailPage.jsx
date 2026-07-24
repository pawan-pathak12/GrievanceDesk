import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../Styles/auth.css';

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
      <AuthBrand />

      <div className="auth-form-panel">
        <div className="auth-form-container">
          <div className="auth-steps">
            <div className="auth-step active"></div>
            <div className="auth-step"></div>
            <div className="auth-step"></div>
          </div>

          <div className="auth-form-header">
            <h2>Create Account</h2>
            <p>Step 1 of 3 — Verify your organization email</p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Organization Email</label>
              <div className="input-wrapper">
                <span className="input-icon">✉️</span>
                <input
                  type="email"
                  placeholder="name@organization.com"
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
            Already have an account? <Link to={ROUTES.LOGIN}>Sign in</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
