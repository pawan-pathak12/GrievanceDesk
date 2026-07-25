import { del, get, patch, post, put } from './client';

export const adminNoticeApi = {
  getAll: () => get('/api/admin/notices'),
  getById: (id) => get(`/api/admin/notices/${id}`),
  create: (data) => post('/api/admin/notices', data),
  update: (id, data) => put(`/api/admin/notices/${id}`, data),
  publish: (id) => patch(`/api/admin/notices/${id}/publish`),
  unpublish: (id) => patch(`/api/admin/notices/${id}/unpublish`),
  delete: (id) => del(`/api/admin/notices/${id}`),
};

export const noticeApi = {
  getPublished: () => get('/api/notices'),
  getById: (id) => get(`/api/notices/${id}`),
};
