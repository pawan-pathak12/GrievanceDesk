import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';

export default function TopBar() {
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();

  const initials = user
    ? `${(user.firstName || '')[0] || ''}${(user.lastName || '')[0] || ''}`.toUpperCase()
    : '?';

  return (
    <div className="topbar">
      <div className="topbar-search">
        <span className="search-icon">🔍</span>
        <input type="text" placeholder="Search complaints, IDs, or categories..." />
      </div>

      <div className="topbar-actions">
        {!isAdmin && (
          <button className="btn-new-ticket" onClick={() => navigate(ROUTES.NEW_COMPLAINT)}>
            + New Ticket
          </button>
        )}
        <button className="topbar-btn" title="Notifications">🔔</button>
        <button className="topbar-btn" title="Settings">⚙️</button>
        <div className="topbar-avatar">{initials}</div>
      </div>
    </div>
  );
}
