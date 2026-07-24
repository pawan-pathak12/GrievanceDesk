// In development, use the Vite proxy so localhost and 127.0.0.1 behave the same
// and the browser does not need to trust the backend's local HTTPS certificate.
export const API_BASE_URL = import.meta.env.VITE_API_URL || (import.meta.env.DEV ? '' : 'https://localhost:7025');
