import { createContext, useCallback, useContext, useState } from 'react';
import { Check, Info, X, XCircle } from 'lucide-react';
import '../Styles/toast.css';

const ToastContext = createContext(null);

let toastId = 0;

const toastDetails = {
  success: { title: 'Success', Icon: Check },
  error: { title: 'Something went wrong', Icon: XCircle },
  info: { title: 'Information', Icon: Info },
};

export function ToastProvider({ children }) {
  const [toasts, setToasts] = useState([]);

  const dismiss = useCallback((id) => {
    setToasts((prev) => prev.filter((toast) => toast.id !== id));
  }, []);

  const addToast = useCallback((message, type = 'success') => {
    const id = ++toastId;
    setToasts((prev) => [...prev, { id, message, type }]);
    setTimeout(() => dismiss(id), 5000);
  }, [dismiss]);

  const success = useCallback((msg) => addToast(msg, 'success'), [addToast]);
  const error = useCallback((msg) => addToast(msg, 'error'), [addToast]);

  return (
    <ToastContext.Provider value={{ success, error, dismiss }}>
      {children}
      <div className="toast-container">
        {toasts.map((toast) => {
          const { title, Icon } = toastDetails[toast.type] ?? toastDetails.info;

          return (
            <div key={toast.id} className={`toast toast-${toast.type}`} role="status">
              <span className="toast-icon" aria-hidden="true">
                <Icon size={19} strokeWidth={2.4} />
              </span>
              <div className="toast-copy">
                <strong>{title}</strong>
                <span className="toast-message">{toast.message}</span>
              </div>
              <button
                type="button"
                className="toast-dismiss"
                onClick={() => dismiss(toast.id)}
                aria-label="Dismiss notification"
              >
                <X size={18} />
              </button>
            </div>
          );
        })}
      </div>
    </ToastContext.Provider>
  );
}

export function useToast() {
  const ctx = useContext(ToastContext);
  if (!ctx) throw new Error('useToast must be inside ToastProvider');
  return ctx;
}
