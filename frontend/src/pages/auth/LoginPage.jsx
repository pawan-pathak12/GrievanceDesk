import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useAuth } from '../../context/AuthContext';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import '../../styles/auth.css';

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
        // Load user profile to get role
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
      <div className="auth-logo">
        <div className="auth-logo-icon">🎓</div>
        <h1>GrievanceDesk</h1>
        <p>Institutional Efficiency & Fairness</p>
      </div>

      <div className="auth-card">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Email</label>
            <div className="input-wrapper">
              <span className="input-icon">👤</span>
              <input
                type="email"
                placeholder="example@gmail.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <label>Password</label>
              <a href="#" className="form-link-inline" style={{ fontSize: '0.85rem' }}>Forgot password?</a>
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
            {loading ? 'Logging in...' : 'Login In →'}
          </button>
        </form>

        <div className="auth-divider">or</div>

        <div className="auth-footer">
          Don't have an account? <Link to={ROUTES.REGISTER_EMAIL}>Register</Link>
        </div>
      </div>

      <div className="page-footer">
        © 2026 GrievanceDesk. Institutional Efficiency & Fairness.
      </div>
    </div>
  );
}
