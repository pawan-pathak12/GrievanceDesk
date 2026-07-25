import { del, get, post, put } from './client';

export const commentApi = {
  getByComplaint: (complaintId) => get(`/api/complaints/${complaintId}/comments`),
  create: (complaintId, comment) => post(`/api/complaints/${complaintId}/comments`, { comment }),
  update: (commentId, comment) => put(`/api/complaints/comments/${commentId}`, { comment }),
  deleteOwn: (commentId) => del(`/api/complaints/comments/${commentId}`),
  deleteAsAdmin: (commentId) => del(`/api/admin/comments/${commentId}`),
};
