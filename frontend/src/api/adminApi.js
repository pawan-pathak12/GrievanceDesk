import { get, post, put } from './client';

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

  updateStatus: (id, newStatus) => put(`/api/admin/complaints/${id}/status`, { newStatus }),

  addNote: (id, noteText) => post(`/api/admin/complaints/${id}/notes`, { noteText }),

  markDuplicate: (id, isDuplicate) => put(`/api/admin/complaints/${id}/duplicate`, { isDuplicate }),

  markIrrelevant: (id, reason) => put(`/api/admin/complaints/${id}/irrelevant`, { reason }),
};
