export const ROUTES = {
  LOGIN: '/login',
  REGISTER_EMAIL: '/register',
  REGISTER_OTP: '/register/verify-otp',
  REGISTER_PASSWORD: '/register/create-password',

  // User
  DASHBOARD: '/dashboard',
  MY_COMPLAINTS: '/my-complaints',
  NEW_COMPLAINT: '/my-complaints/new',
  COMPLAINT_DETAIL: '/my-complaints/:id',
  NOTICES: '/notices',
  NOTICE_DETAIL: '/notices/:id',

  // Admin
  ADMIN_DASHBOARD: '/admin/dashboard',
  ADMIN_COMPLAINTS: '/admin/complaints',
  ADMIN_COMPLAINT_DETAIL: '/admin/complaints/:id',
  ADMIN_NOTICES: '/admin/notices',
  ADMIN_NOTICE_NEW: '/admin/notices/new',
  ADMIN_NOTICE_DETAIL: '/admin/notices/:id',
  ADMIN_NOTICE_EDIT: '/admin/notices/:id/edit',
};
