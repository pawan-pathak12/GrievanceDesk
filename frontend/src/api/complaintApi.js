import { get, post, put, del } from './client';

export const complaintApi = {
  create: (data) => post('/api/complaints', data),

  getMy: () => get('/api/complaints/my'),

  getById: (id) => get(`/api/complaints/${id}`),

  update: (id, data) => put(`/api/complaints/${id}`, data),

  delete: (id) => del(`/api/complaints/${id}`),

  getMyIrrelevant: () => get('/api/complaints/my/irrelevant'),
};
