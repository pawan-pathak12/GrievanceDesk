import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../Styles/auth.css';

function MailIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <rect x="3" y="5" width="18" height="14" rx="2" />
      <path d="m3 7 9 6 9-6" />
    </svg>
  );
}

function LockIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <rect x="5" y="10" width="14" height="11" rx="2" />
      <path d="M8 10V7a4 4 0 0 1 8 0v3" />
    </svg>
  );
}

function EyeIcon({ hidden }) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      {hidden ? (
        <>
          <path d="m3 3 18 18" />
          <path d="M10.6 10.6a2 2 0 0 0 2.8 2.8" />
          <path d="M9.9 5.2A10.8 10.8 0 0 1 12 5c6.5 0 10 7 10 7a18.3 18.3 0 0 1-3.1 3.8" />
          <path d="M6.2 6.2C3.7 8 2 12 2 12s3.5 7 10 7a10.8 10.8 0 0 0 2.1-.2" />
        </>
      ) : (
        <>
          <path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6S2 12 2 12Z" />
          <circle cx="12" cy="12" r="2.5" />
        </>
      )}
    </svg>
  );
}

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPass, setShowPass] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();
  const toast = useToast();

  async function handleSubmit(e) {
    e.preventDefault();
    if (!email || !password) return;

    setLoading(true);
    const res = await authApi.login(email, password);

    if (res.isSuccess) {
      const token = res.data?.accessToken || res.data?.token;
      if (token) {
        login(token, null);
        const meRes = await authApi.getMe();
        if (meRes.isSuccess) {
          login(token, meRes.data);
          const role = meRes.data?.role;
          navigate(role === 'Admin' || role === 0 ? ROUTES.ADMIN_DASHBOARD : ROUTES.DASHBOARD);
          toast.success('Logged in successfully');
        } else {
          toast.error(meRes.errors?.[0] || 'Unable to load profile');
        }
      } else {
        toast.error('Login response did not include an access token');
      }
    } else {
      toast.error(res.errors?.[0] || 'Login failed');
    }

    setLoading(false);
  }

  return (
    <div className="auth-page">
      <AuthBrand />

      <div className="auth-form-panel">
        <div className="auth-form-container">
          <div className="auth-form-header">
            <h2>Welcome back</h2>
            <p>Sign in to your GrievanceDesk account</p>
          </div>

          <form className="login-form" onSubmit={handleSubmit}>
            <div className="form-group">
              <label htmlFor="login-email">Email Address</label>
              <div className="input-wrapper">
                <span className="login-input-icon"><MailIcon /></span>
                <input
                  id="login-email"
                  type="email"
                  placeholder="name@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="login-password">Password</label>
              <div className="input-wrapper">
                <span className="login-input-icon"><LockIcon /></span>
                <input
                  id="login-password"
                  type={showPass ? 'text' : 'password'}
                  placeholder="Enter your password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
                <button
                  type="button"
                  className="login-password-toggle"
                  onClick={() => setShowPass(!showPass)}
                  aria-label={showPass ? 'Hide password' : 'Show password'}
                  title={showPass ? 'Hide password' : 'Show password'}
                >
                  <EyeIcon hidden={showPass} />
                </button>
              </div>
              <Link className="login-forgot-link" to="/forgot-password">Forgot password?</Link>
            </div>

            <button type="submit" className="btn-primary login-submit" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In'}
            </button>
          </form>

          <div className="auth-footer login-register-section">
            Do not have an account? <Link to={ROUTES.REGISTER_EMAIL}>Create account</Link>
          </div>

          <div className="auth-copyright">
            Copyright 2026 GrievanceDesk. All rights reserved.
          </div>
        </div>
      </div>
    </div>
  );
}
