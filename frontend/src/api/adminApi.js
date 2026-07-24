import { get, put } from './client';

export const adminApi = {
  getAll: (params = {}) => {
    const query = new URLSearchParams();
    if (params.status !== undefined && params.status !== null) query.append('status', params.status);
    if (params.category !== undefined && params.category !== null) query.append('category', params.category);
    if (params.search) query.append('search', params.search);
    const qs = query.toString();
    return get(`/api/admin/complaints${qs ? `?${qs}` : ''}`);
  },

  getIrrelevant: () => get('/api/admin/complaints/irrelevant'),

  getById: (id) => get(`/api/admin/complaints/${id}`),

  updateComplaint: (id, data) => put(`/api/admin/complaints/${id}`, data),
};
