import { API_BASE_URL } from './config';

/**
 * Centralized fetch wrapper.
 * - Attaches JWT from localStorage
 * - Parses JSON response
 * - Returns { isSuccess, statusCode, data, errors }
 */
export async function apiRequest(path, options = {}) {
  const token = localStorage.getItem('token');

  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers,
  };

  try {
    const res = await fetch(`${API_BASE_URL}${path}`, {
      ...options,
      headers,
    });

    // Handle no-content responses
    if (res.status === 204) {
      return { isSuccess: true, statusCode: 204, data: null, errors: null };
    }

    const json = await res.json();

    return {
      isSuccess: json.isSuccess ?? res.ok,
      statusCode: json.statusCode ?? res.status,
      data: json.data ?? null,
      errors: json.errors ?? null,
    };
  } catch (err) {
    return {
      isSuccess: false,
      statusCode: 0,
      data: null,
      errors: ['Network error. Please check your connection.'],
    };
  }
}

export function get(path) {
  return apiRequest(path, { method: 'GET' });
}

export function post(path, body) {
  return apiRequest(path, { method: 'POST', body: JSON.stringify(body) });
}

export function put(path, body) {
  return apiRequest(path, { method: 'PUT', body: JSON.stringify(body) });
}

export function patch(path, body) {
  return apiRequest(path, {
    method: 'PATCH',
    ...(body === undefined ? {} : { body: JSON.stringify(body) }),
  });
}

export function del(path) {
  return apiRequest(path, { method: 'DELETE' });
}
