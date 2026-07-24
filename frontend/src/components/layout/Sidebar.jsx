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
          <span>{isAdmin ? 'Admin Dashboard' : 'User Dashboard'}</span>
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
          <li>
            <NavLink
              to={ROUTES.MY_COMPLAINTS}
              className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
            >
              <span className="nav-icon">📋</span>
              My Complaints
            </NavLink>
          </li>
          {isAdmin && (
            <li>
              <NavLink
                to={ROUTES.ADMIN_COMPLAINTS}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon">⚙️</span>
                Admin Panel
              </NavLink>
            </li>
          )}
        </ul>
      </div>

      <div className="sidebar-bottom">
        <button className="sidebar-link" onClick={handleLogout} style={{ width: '100%', border: 'none', background: 'none', textAlign: 'left' }}>
          <span className="nav-icon">🚪</span>
          Logout
        </button>
      </div>
    </aside>
  );
}
