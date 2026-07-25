import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';
import { authApi } from '../../api/authApi';
import { useToast } from '../../context/ToastContext';
import { ROUTES } from '../../utils/routes';
import AuthBrand from '../../components/auth/AuthBrand';
import OtpCodeInput from '../../components/auth/OtpCodeInput';
import '../../Styles/auth.css';

export default function VerifyOtpPage() {
  const [otp, setOtp] = useState(['', '', '', '', '', '']);
  const [loading, setLoading] = useState(false);
  const [resending, setResending] = useState(false);
  const [secondsRemaining, setSecondsRemaining] = useState(60);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const email = location.state?.email;
  const isForgot = location.state?.forgot;
  const hasIncompleteCode = otp.some(Boolean) && otp.join('').length < 6;
  const validationMessage = error || (hasIncompleteCode ? 'Enter all 6 digits of the verification code.' : '');

  useEffect(() => {
    if (!email) navigate(ROUTES.REGISTER_EMAIL, { replace: true });
  }, [email, navigate]);

  useEffect(() => {
    if (secondsRemaining === 0) {
      return undefined;
    }

    const countdown = window.setInterval(() => {
      setSecondsRemaining((seconds) => Math.max(seconds - 1, 0));
    }, 1000);

    return () => window.clearInterval(countdown);
  }, [secondsRemaining]);

  function handleOtpChange(nextOtp) {
    setOtp(nextOtp);
    if (error) {
      setError('');
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    const code = otp.join('');
    if (code.length !== 6) {
      setError('Enter all 6 digits of the verification code.');
      return;
    }

    setLoading(true);
    setError('');
    const res = await authApi.verifyOtp(email, code);
    setLoading(false);

    if (res.isSuccess) {
      toast.success('Code verified');
      const verificationToken = res.data?.verificationToken;
      if (isForgot) {
        navigate('/forgot-password/reset', { state: { email, verificationToken } });
      } else {
        navigate(ROUTES.REGISTER_PASSWORD, { state: { email, verificationToken } });
      }
    } else {
      setError(res.errors?.[0] || 'Invalid verification code.');
    }
  }

  async function handleResend() {
    if (secondsRemaining > 0 || resending) return;

    setResending(true);
    const res = await authApi.checkEmail(email);
    setResending(false);

    if (res.isSuccess) {
      setOtp(['', '', '', '', '', '']);
      setError('');
      setSecondsRemaining(60);
      toast.success('Code resent to your email');
    } else {
      toast.error(res.errors?.[0] || 'Failed to resend');
    }
  }

  return (
    <div className="auth-page">
      <AuthBrand />

      <div className="auth-form-panel">
        <div className="auth-form-container">
          <button type="button" className="auth-back-link" onClick={() => navigate(ROUTES.REGISTER_EMAIL)} disabled={loading}>
            <ArrowLeft size={16} aria-hidden="true" />
            Back
          </button>

          <div className="auth-steps">
            <div className="auth-step done"></div>
            <div className="auth-step active"></div>
            <div className="auth-step"></div>
          </div>

          <div className="auth-form-header">
            <h2>Verify your email</h2>
            <p>Step 2 of 3 - Enter the 6-digit code sent to <strong>{email}</strong></p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="otp-field">
              <OtpCodeInput value={otp} onChange={handleOtpChange} disabled={loading} invalid={Boolean(validationMessage)} />
              {validationMessage && <p className="otp-error" role="alert">{validationMessage}</p>}
            </div>

            <button type="submit" className="btn-primary" disabled={loading || otp.join('').length !== 6}>
              {loading ? 'Verifying...' : 'Verify Code'}
            </button>
          </form>

          <div className="resend-link">
            <span>Did not receive a code?</span>
            {secondsRemaining > 0 ? (
              <button type="button" className="resend-countdown" disabled>
                Resend available in 00:{String(secondsRemaining).padStart(2, '0')}
              </button>
            ) : (
              <button type="button" onClick={handleResend} disabled={resending}>
                {resending ? 'Resending...' : 'Resend Code'}
              </button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
