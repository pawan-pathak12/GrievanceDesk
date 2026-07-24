import { useEffect, useRef } from 'react';

const OTP_LENGTH = 6;

export default function OtpCodeInput({ value, onChange, disabled = false, invalid = false }) {
  const inputRefs = useRef([]);

  useEffect(() => {
    if (!disabled) {
      inputRefs.current[0]?.focus();
    }
  }, [disabled]);

  function updateValue(nextValue, focusIndex) {
    onChange(nextValue);
    if (focusIndex !== undefined) {
      requestAnimationFrame(() => inputRefs.current[focusIndex]?.focus());
    }
  }

  function handleChange(index, rawValue) {
    const digits = rawValue.replace(/\D/g, '');
    const nextValue = [...value];

    if (!digits) {
      nextValue[index] = '';
      updateValue(nextValue);
      return;
    }

    digits.slice(0, OTP_LENGTH - index).split('').forEach((digit, offset) => {
      nextValue[index + offset] = digit;
    });

    const nextIndex = Math.min(index + digits.length, OTP_LENGTH - 1);
    updateValue(nextValue, nextIndex);
  }

  function handleKeyDown(index, event) {
    if (event.key === 'Backspace' && !value[index] && index > 0) {
      event.preventDefault();
      inputRefs.current[index - 1]?.focus();
    }
  }

  function handlePaste(event) {
    event.preventDefault();
    const digits = event.clipboardData.getData('text').replace(/\D/g, '').slice(0, OTP_LENGTH);

    if (!digits) {
      return;
    }

    const nextValue = Array(OTP_LENGTH).fill('');
    digits.split('').forEach((digit, index) => {
      nextValue[index] = digit;
    });

    updateValue(nextValue, Math.min(digits.length, OTP_LENGTH - 1));
  }

  return (
    <div className="otp-inputs" onPaste={handlePaste}>
      {value.map((digit, index) => (
        <input
          key={index}
          ref={(element) => {
            inputRefs.current[index] = element;
          }}
          aria-label={`Verification code digit ${index + 1}`}
          aria-invalid={invalid}
          autoComplete={index === 0 ? 'one-time-code' : 'off'}
          disabled={disabled}
          inputMode="numeric"
          maxLength={1}
          pattern="[0-9]*"
          type="text"
          value={digit}
          onChange={(event) => handleChange(index, event.target.value)}
          onKeyDown={(event) => handleKeyDown(index, event)}
        />
      ))}
    </div>
  );
}
