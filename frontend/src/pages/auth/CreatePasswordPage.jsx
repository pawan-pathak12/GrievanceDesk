import { useState, useEffect } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import '../../styles/auth.css';

export default function CreatePasswordPage() {
  const [form, setForm] = useState({ firstName: '', lastName: '', password: '', confirmPassword: '' });
  const [showPass, setShowPass] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const email = location.state?.email;

  useEffect(() => {
    if (!email) navigate(ROUTES.REGISTER_EMAIL, { replace: true });
  }, [email, navigate]);

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (form.password !== form.confirmPassword) {
      toast.error('Passwords do not match');
      return;
    }

    setLoading(true);
    const res = await authApi.createPassword({
      email,
      firstName: form.firstName,
      lastName: form.lastName,
      password: form.password,
      confirmPassword: form.confirmPassword,
    });
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Registration complete! Please login.');
      navigate(ROUTES.LOGIN);
    } else {
      toast.error(res.errors?.[0] || 'Registration failed');
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
          <div className="form-row">
            <div className="form-group">
              <label>First Name</label>
              <div className="input-wrapper">
                <span className="input-icon">👤</span>
                <input type="text" name="firstName" placeholder="John" value={form.firstName} onChange={handleChange} required />
              </div>
            </div>
            <div className="form-group">
              <label>Last Name</label>
              <div className="input-wrapper">
                <span className="input-icon">👤</span>
                <input type="text" name="lastName" placeholder="Doe" value={form.lastName} onChange={handleChange} required />
              </div>
            </div>
          </div>

          <div className="form-group">
            <label>Password</label>
            <div className="input-wrapper">
              <span className="input-icon">🔒</span>
              <input type={showPass ? 'text' : 'password'} name="password" placeholder="••••••••" value={form.password} onChange={handleChange} required />
              <button type="button" className="toggle-password" onClick={() => setShowPass(!showPass)}>
                {showPass ? '🙈' : '👁️'}
              </button>
            </div>
          </div>

          <div className="form-group">
            <label>Confirm Password</label>
            <div className="input-wrapper">
              <span className="input-icon">🔒</span>
              <input type={showConfirm ? 'text' : 'password'} name="confirmPassword" placeholder="••••••••" value={form.confirmPassword} onChange={handleChange} required />
              <button type="button" className="toggle-password" onClick={() => setShowConfirm(!showConfirm)}>
                {showConfirm ? '🙈' : '👁️'}
              </button>
            </div>
          </div>

          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Creating account...' : 'Complete Registration →'}
          </button>
        </form>

        <div className="auth-divider" />

        <div className="auth-footer">
          Already have an account? <Link to={ROUTES.LOGIN}>Back to Login</Link>
        </div>
      </div>
    </div>
  );
}
