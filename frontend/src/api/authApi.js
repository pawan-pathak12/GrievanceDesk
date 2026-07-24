import { post, get } from './client';

export const authApi = {
  checkEmail: (email) => post('/api/auth/register/check-email', { email }),

  verifyOtp: (email, code) => post('/api/auth/register/verify-otp', { email, code }),

  createPassword: (data) => post('/api/auth/register/create-password', data),

  requestPasswordReset: (email) => post('/api/auth/forgot-password/check-email', { email }),

  verifyPasswordResetOtp: (email, code) => post('/api/auth/forgot-password/verify-otp', { email, code }),

  resetPassword: (data) => post('/api/auth/forgot-password/reset', data),

  login: (email, password) => post('/api/auth/login', { email, password }),

  changePassword: (data) => post('/api/auth/change-password', data),

  getMe: () => get('/api/auth/me'),
};
