import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../Styles/auth.css';

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const toast = useToast();

  async function handleSubmit(e) {
    e.preventDefault();
    if (!email) return;

    setLoading(true);
    const res = await authApi.requestPasswordReset(email);
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Verification code sent to your email');
      navigate('/forgot-password/verify', { state: { email, forgot: true } });
    } else {
      toast.error(res.errors?.[0] || 'Unable to send reset code');
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
            <h2>Forgot Password</h2>
            <p>Step 1 of 3 - Enter your email to receive a reset code</p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Email Address</label>
              <div className="input-wrapper">
                <span className="input-icon">EM</span>
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
              {loading ? 'Sending...' : 'Send Reset Code'}
            </button>
          </form>

          <div className="auth-divider">or</div>

          <div className="auth-footer">
            Remember your password? <Link to={ROUTES.LOGIN}>Sign in</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
