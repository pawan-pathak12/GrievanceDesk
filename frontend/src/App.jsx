import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ToastProvider } from './context/ToastContext';
import ProtectedRoute from './components/common/ProtectedRoute';
import AppLayout from './components/layout/AppLayout';
import { ROUTES } from './utils/routes';

// Auth pages
import LoginPage from './pages/auth/LoginPage';
import CheckEmailPage from './pages/auth/CheckEmailPage';
import VerifyOtpPage from './pages/auth/VerifyOtpPage';
import CreatePasswordPage from './pages/auth/CreatePasswordPage';

// User pages
import DashboardPage from './pages/user/DashboardPage';
import MyComplaintsPage from './pages/user/MyComplaintsPage';
import NewComplaintPage from './pages/user/NewComplaintPage';
import ComplaintDetailPage from './pages/user/ComplaintDetailPage';

// Admin pages
import AdminDashboardPage from './pages/admin/AdminDashboardPage';
import AdminComplaintsPage from './pages/admin/AdminComplaintsPage';
import AdminComplaintDetailPage from './pages/admin/AdminComplaintDetailPage';

import './styles/global.css';
import './styles/layout.css';
import './styles/components.css';

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastProvider>
          <Routes>
            {/* Public - Auth */}
            <Route path={ROUTES.LOGIN} element={<LoginPage />} />
            <Route path={ROUTES.REGISTER_EMAIL} element={<CheckEmailPage />} />
            <Route path={ROUTES.REGISTER_OTP} element={<VerifyOtpPage />} />
            <Route path={ROUTES.REGISTER_PASSWORD} element={<CreatePasswordPage />} />

            {/* Protected - App Layout */}
            <Route element={
              <ProtectedRoute>
                <AppLayout />
              </ProtectedRoute>
            }>
              {/* User routes */}
              <Route path={ROUTES.DASHBOARD} element={<DashboardPage />} />
              <Route path={ROUTES.MY_COMPLAINTS} element={<MyComplaintsPage />} />
              <Route path={ROUTES.NEW_COMPLAINT} element={<NewComplaintPage />} />
              <Route path={ROUTES.COMPLAINT_DETAIL} element={<ComplaintDetailPage />} />

              {/* Admin routes */}
              <Route path={ROUTES.ADMIN_DASHBOARD} element={
                <ProtectedRoute allowedRoles={['Admin', 0]}>
                  <AdminDashboardPage />
                </ProtectedRoute>
              } />
              <Route path={ROUTES.ADMIN_COMPLAINTS} element={
                <ProtectedRoute allowedRoles={['Admin', 0]}>
                  <AdminComplaintsPage />
                </ProtectedRoute>
              } />
              <Route path={ROUTES.ADMIN_COMPLAINT_DETAIL} element={
                <ProtectedRoute allowedRoles={['Admin', 0]}>
                  <AdminComplaintDetailPage />
                </ProtectedRoute>
              } />
            </Route>

            {/* Fallback */}
            <Route path="/" element={<Navigate to={ROUTES.LOGIN} replace />} />
            <Route path="*" element={<Navigate to={ROUTES.LOGIN} replace />} />
          </Routes>
        </ToastProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}
