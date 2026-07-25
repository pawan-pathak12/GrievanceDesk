import { useAuth } from '../../context/AuthContext';

export default function TopBar() {
  const { user, isAdmin } = useAuth();

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
        <div className="topbar-avatar">{initials || 'U'}</div>
      </div>
    </div>
  );
}
