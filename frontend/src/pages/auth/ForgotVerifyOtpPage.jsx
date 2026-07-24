import { useState, useRef, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import '../../styles/auth.css';

export default function ForgotVerifyOtpPage() {
  const [otp, setOtp] = useState(['', '', '', '', '', '']);
  const [loading, setLoading] = useState(false);
  const inputRefs = useRef([]);
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const email = location.state?.email;

  useEffect(() => {
    if (!email) navigate('/forgot-password', { replace: true });
  }, [email, navigate]);

  function handleChange(index, value) {
    if (value.length > 1) value = value[0];
    const newOtp = [...otp];
    newOtp[index] = value;
    setOtp(newOtp);
    if (value && index < 5) inputRefs.current[index + 1]?.focus();
  }

  function handleKeyDown(index, e) {
    if (e.key === 'Backspace' && !otp[index] && index > 0) {
      inputRefs.current[index - 1]?.focus();
    }
  }

  function handlePaste(e) {
    e.preventDefault();
    const paste = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, 6);
    if (paste.length === 6) {
      setOtp(paste.split(''));
      inputRefs.current[5]?.focus();
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const code = otp.join('');
    if (code.length !== 6) return;

    setLoading(true);
    const res = await authApi.verifyOtp(email, code);
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Code verified successfully');
      navigate('/forgot-password/reset', { state: { email } });
    } else {
      toast.error(res.errors?.[0] || 'Invalid code');
    }
  }

  async function handleResend() {
    const res = await authApi.checkEmail(email);
    if (res.isSuccess) toast.success('Code resent to your email');
    else toast.error(res.errors?.[0] || 'Failed to resend');
  }

  return (
    <div className="auth-page">
      <AuthBrand />

      <div className="auth-form-panel">
        <div className="auth-form-container">
          <div className="auth-steps">
            <div className="auth-step done"></div>
            <div className="auth-step active"></div>
            <div className="auth-step"></div>
          </div>

          <div className="auth-form-header">
            <h2>Verify Code</h2>
            <p>Step 2 of 3 — Enter the 6-digit code sent to <strong>{email}</strong></p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="otp-inputs" onPaste={handlePaste}>
              {otp.map((digit, i) => (
                <input
                  key={i}
                  ref={(el) => (inputRefs.current[i] = el)}
                  type="text"
                  inputMode="numeric"
                  maxLength={1}
                  value={digit}
                  onChange={(e) => handleChange(i, e.target.value)}
                  onKeyDown={(e) => handleKeyDown(i, e)}
                  autoFocus={i === 0}
                />
              ))}
            </div>

            <button type="submit" className="btn-primary" disabled={loading || otp.join('').length !== 6}>
              {loading ? 'Verifying...' : 'Verify & Continue →'}
            </button>
          </form>

          <div className="resend-link">
            Didn't receive a code?{' '}
            <button onClick={handleResend}>Resend code</button>
          </div>
        </div>
      </div>
    </div>
  );
}
