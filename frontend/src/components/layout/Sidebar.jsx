import { useEffect, useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';

export default function Sidebar() {
  const { user, logout, isAdmin } = useAuth();
  const [isLogoutDialogOpen, setIsLogoutDialogOpen] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    if (!isLogoutDialogOpen) return undefined;

    function handleKeyDown(event) {
      if (event.key === 'Escape') setIsLogoutDialogOpen(false);
    }

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [isLogoutDialogOpen]);

  function handleLogout() {
    setIsLogoutDialogOpen(true);
  }

  function confirmLogout() {
    logout();
    navigate(ROUTES.LOGIN);
  }

  const displayName = [user?.firstName, user?.lastName].filter(Boolean).join(' ') || 'Signed in user';

  return (
    <>
      <aside className="sidebar">
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">GD</div>
        <div>
          <h2>GrievanceDesk</h2>
          <span>{isAdmin ? 'Admin Portal' : 'Member Portal'}</span>
        </div>
      </div>

      <div className="sidebar-section">
        <div className="sidebar-section-label">Workspace</div>
        <ul className="sidebar-nav">
          <li>
            <NavLink
              to={isAdmin ? ROUTES.ADMIN_DASHBOARD : ROUTES.DASHBOARD}
              className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
            >
              <span className="nav-icon">DB</span>
              Dashboard
            </NavLink>
          </li>

          {!isAdmin && (
            <li>
              <NavLink
                to={ROUTES.MY_COMPLAINTS}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon">MC</span>
                My Complaints
              </NavLink>
            </li>
          )}

          {isAdmin && (
            <li>
              <NavLink
                to={ROUTES.ADMIN_COMPLAINTS}
                className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}
              >
                <span className="nav-icon">AC</span>
                All Complaints
              </NavLink>
            </li>
          )}
        </ul>
      </div>

      <div className="sidebar-bottom">
        <div className="sidebar-user">
          <div className="sidebar-avatar">{(user?.firstName?.[0] || 'U').toUpperCase()}</div>
          <div className="sidebar-user-info">
            <span className="sidebar-user-name">{displayName}</span>
            <span className="sidebar-user-role">{isAdmin ? 'Administrator' : 'Organization member'}</span>
          </div>
        </div>
        <button className="sidebar-link logout-btn" onClick={handleLogout}>
          <span className="nav-icon">SO</span>
          Sign Out
        </button>
      </div>
      </aside>

      {isLogoutDialogOpen && (
        <div
          className="logout-modal-backdrop"
          role="presentation"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) setIsLogoutDialogOpen(false);
          }}
        >
          <section className="logout-modal" role="dialog" aria-modal="true" aria-labelledby="logout-modal-title">
            <h2 id="logout-modal-title">Sign out?</h2>
            <p>You will need to sign in again to access your workspace.</p>
            <div className="logout-modal-actions">
              <button type="button" className="logout-modal-cancel" onClick={() => setIsLogoutDialogOpen(false)} autoFocus>
                Cancel
              </button>
              <button type="button" className="logout-modal-confirm" onClick={confirmLogout}>
                Sign out
              </button>
            </div>
          </section>
        </div>
      )}
    </>
  );
}
