import { get } from './client';

export const dashboardApi = {
  getUser: () => get('/api/dashboard/user'),

  getAdmin: () => get('/api/dashboard/admin'),
};
