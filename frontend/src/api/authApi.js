import { post, get } from './client';

export const authApi = {
  checkEmail: (email) => post('/api/auth/register/check-email', { email }),

  verifyOtp: (email, otpCode) => post('/api/auth/register/verify-otp', { email, otpCode }),

  createPassword: (data) => post('/api/auth/register/create-password', data),

  login: (email, password) => post('/api/auth/login', { email, password }),

  changePassword: (data) => post('/api/auth/change-password', data),

  getMe: () => get('/api/auth/me'),
};
