import { createContext, useContext, useState, useCallback } from 'react';
import '../Styles/toast.css';

const ToastContext = createContext(null);

let toastId = 0;

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  const dismiss = useCallback((id) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
  }, []);

  const addToast = useCallback((message, type = 'success', options = {}) => {
    const id = ++toastId;
    setToasts((prev) => [...prev, { id, message, type, onConfirm: options.onConfirm }]);
    setTimeout(() => dismiss(id), options.duration || 5000);
  }, [dismiss]);

  const success = useCallback((msg) => addToast(msg, 'success'), [addToast]);
  const error = useCallback((msg) => addToast(msg, 'error'), [addToast]);
  const confirm = useCallback(
    (message, onConfirm) => addToast(message, 'confirm', { onConfirm, duration: 8000 }),
    [addToast],
  );

  return (
    <ToastContext.Provider value={{ success, error, confirm, dismiss }}>
      {children}
      <div className="toast-container">
        {toasts.map((t) => (
          <div key={t.id} className={`toast toast-${t.type}`}>
            <span className="toast-icon">{t.type === 'success' ? '✓' : '✕'}</span>
            <span className="toast-message">{t.message}</span>
            {t.type === 'confirm' && (
              <div className="toast-actions">
                <button type="button" className="toast-action toast-action-cancel" onClick={() => dismiss(t.id)}>
                  Cancel
                </button>
                <button type="button" className="toast-action toast-action-confirm" onClick={() => {
                  dismiss(t.id);
                  t.onConfirm?.();
                }}>
                  Sign out
                </button>
              </div>
            )}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('useToast must be inside ToastProvider');
  return ctx;
}
