import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';

export default function TopBar() {
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();

  const initials = user
    ? `${(user.firstName || '')[0] || ''}${(user.lastName || '')[0] || ''}`.toUpperCase()
    : 'U';

  return (
    <div className="topbar">
      <div className="topbar-title">
        <span>{isAdmin ? 'Admin workspace' : 'Complaint workspace'}</span>
        <strong>{user?.firstName ? `Hello, ${user.firstName}` : 'Welcome back'}</strong>
      </div>

      <div className="topbar-actions">
        {!isAdmin && (
          <button className="btn-new-ticket" onClick={() => navigate(ROUTES.NEW_COMPLAINT)}>
            New Complaint
          </button>
        )}
        <div className="topbar-avatar">{initials || 'U'}</div>
      </div>
    </div>
  );
}
