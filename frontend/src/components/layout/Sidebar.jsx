import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';

export default function Sidebar() {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate(ROUTES.LOGIN);
  }

  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">🎓</div>
        <div>
          <h2>GrievanceDesk</h2>
          <span>{isAdmin ? 'Admin Portal' : 'User Portal'}</span>
        </div>
      </div>

      <div className="sidebar-section">
        <div className="sidebar-section-label">General</div>
        <ul className="sidebar-nav">
          <li>
            <NavLink
              to={isAdmin ? ROUTES.ADMIN_DASHBOARD : ROUTES.DASHBOARD}
              className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
            >
              <span className="nav-icon">📊</span>
              Dashboard
            </NavLink>
          </li>

          {/* My Complaints — User only */}
          {!isAdmin && (
            <li>
              <NavLink
                to={ROUTES.MY_COMPLAINTS}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon">📋</span>
                My Complaints
              </NavLink>
            </li>
          )}

          {/* Admin Panel — Admin only */}
          {isAdmin && (
            <li>
              <NavLink
                to={ROUTES.ADMIN_COMPLAINTS}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon">⚙️</span>
                All Complaints
              </NavLink>
            </li>
          )}
        </ul>
      </div>

      <div className="sidebar-bottom">
        <div className="sidebar-user">
          <div className="sidebar-avatar">
            {(user?.firstName?.[0] || '?').toUpperCase()}
          </div>
          <div className="sidebar-user-info">
            <span className="sidebar-user-name">{user?.firstName} {user?.lastName}</span>
            <span className="sidebar-user-role">{isAdmin ? 'Administrator' : 'User'}</span>
          </div>
        </div>
        <button className="sidebar-link logout-btn" onClick={handleLogout}>
          <span className="nav-icon">🚪</span>
          Sign Out
        </button>
      </div>
    </aside>
  );
}
