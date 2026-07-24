import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { ROUTES } from '../../utils/routes';

export default function ProtectedRoute({ children, allowedRoles }) {
  const { user, loading } = useAuth();

  if (loading) {
    return <div className="page-loading">Loading...</div>;
  }

  if (!user) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }

  if (allowedRoles && !allowedRoles.includes(user.role)) {
    // Redirect to their correct dashboard
    const role = user.role === 'Admin' || user.role === 0 ? 'admin' : 'user';
    return <Navigate to={role === 'admin' ? ROUTES.ADMIN_DASHBOARD : ROUTES.DASHBOARD} replace />;
  }

  return children;
}
