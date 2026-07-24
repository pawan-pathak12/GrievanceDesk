import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../Styles/auth.css';

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
    setLoading(false);

    if (res.isSuccess) {
      const token = res.data?.accessToken || res.data?.token;
      if (token) {
        login(token, null);
        const meRes = await authApi.getMe();
        if (meRes.isSuccess) {
          login(token, meRes.data);
          const role = meRes.data?.role;
          if (role === 'Admin' || role === 0) {
            navigate(ROUTES.ADMIN_DASHBOARD);
          } else {
            navigate(ROUTES.DASHBOARD);
          }
        }
      }
      toast.success('Logged in successfully');
    } else {
      toast.error(res.errors?.[0] || 'Login failed');
    }
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

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Email Address</label>
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

            <div className="form-group">
              <div className="form-label-row">
                <label>Password</label>
                <Link to="/forgot-password">Forgot password?</Link>
              </div>
              <div className="input-wrapper">
                <span className="input-icon">🔒</span>
                <input
                  type={showPass ? 'text' : 'password'}
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
                <button type="button" className="toggle-password" onClick={() => setShowPass(!showPass)}>
                  {showPass ? '🙈' : '👁️'}
                </button>
              </div>
            </div>

            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Signing in...' : 'Sign In →'}
            </button>
          </form>

          <div className="auth-divider">or</div>

          <div className="auth-footer">
            Don't have an account? <Link to={ROUTES.REGISTER_EMAIL}>Create account</Link>
          </div>

          <div className="auth-copyright">
            © 2026 GrievanceDesk. All rights reserved.
          </div>
        </div>
      </div>
    </div>
  );
}
