import { useState, useEffect } from 'react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../Styles/auth.css';

export default function CreatePasswordPage() {
  const [form, setForm] = useState({ firstName: '', lastName: '', password: '', confirmPassword: '' });
  const [showPass, setShowPass] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const email = location.state?.email;
  const verificationToken = location.state?.verificationToken;

  useEffect(() => {
    if (!email || !verificationToken) navigate(ROUTES.REGISTER_EMAIL, { replace: true });
  }, [email, verificationToken, navigate]);

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (form.password !== form.confirmPassword) {
      toast.error('Passwords do not match');
      return;
    }
    if (form.password.length < 6) {
      toast.error('Password must be at least 6 characters');
      return;
    }

    setLoading(true);
    const res = await authApi.createPassword({
      email,
      verificationToken,
      firstName: form.firstName,
      lastName: form.lastName,
      password: form.password,
    });
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Account created. Please sign in.');
      navigate(ROUTES.LOGIN);
    } else {
      toast.error(res.errors?.[0] || 'Registration failed');
    }
  }

  return (
    <div className="auth-page">
      <AuthBrand />

      <div className="auth-form-panel">
        <div className="auth-form-container">
          <div className="auth-steps">
            <div className="auth-step done"></div>
            <div className="auth-step done"></div>
            <div className="auth-step active"></div>
          </div>

          <div className="auth-form-header">
            <h2>Complete setup</h2>
            <p>Step 3 of 3 - Set your name and password</p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="form-row">
              <div className="form-group">
                <label>First Name</label>
                <div className="input-wrapper">
                  <span className="input-icon">FN</span>
                  <input type="text" name="firstName" placeholder="John" value={form.firstName} onChange={handleChange} required />
                </div>
              </div>
              <div className="form-group">
                <label>Last Name</label>
                <div className="input-wrapper">
                  <span className="input-icon">LN</span>
                  <input type="text" name="lastName" placeholder="Doe" value={form.lastName} onChange={handleChange} required />
                </div>
              </div>
            </div>

            <div className="form-group">
              <label>Password</label>
              <div className="input-wrapper">
                <span className="input-icon">PW</span>
                <input type={showPass ? 'text' : 'password'} name="password" placeholder="Minimum 6 characters" value={form.password} onChange={handleChange} required />
                <button type="button" className="toggle-password" onClick={() => setShowPass(!showPass)}>
                  {showPass ? 'Hide' : 'Show'}
                </button>
              </div>
            </div>

            <div className="form-group">
              <label>Confirm Password</label>
              <div className="input-wrapper">
                <span className="input-icon">PW</span>
                <input type={showConfirm ? 'text' : 'password'} name="confirmPassword" placeholder="Re-enter password" value={form.confirmPassword} onChange={handleChange} required />
                <button type="button" className="toggle-password" onClick={() => setShowConfirm(!showConfirm)}>
                  {showConfirm ? 'Hide' : 'Show'}
                </button>
              </div>
            </div>

            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Creating account...' : 'Complete Registration'}
            </button>
          </form>

          <div className="auth-divider" />

          <div className="auth-footer">
            Already have an account? <Link to={ROUTES.LOGIN}>Sign in</Link>
          </div>
        </div>
      </div>
    </div>
  );
}
