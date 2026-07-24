import { createContext, useContext, useState, useEffect } from 'react';
import { authApi } from '../api/authApi';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      loadUser();
    } else {
      setLoading(false);
    }
  }, []);

  async function loadUser() {
    const res = await authApi.getMe();
    if (res.isSuccess) {
      setUser(res.data);
    } else {
      localStorage.removeItem('token');
    }
    setLoading(false);
  }

  function login(token, userData) {
    localStorage.setItem('token', token);
    setUser(userData);
  }

  function logout() {
    localStorage.removeItem('token');
    setUser(null);
  }

  const isAdmin = user?.role === 'Admin' || user?.role === 0;
  const isUser = user?.role === 'User' || user?.role === 1;

  return (
    <AuthContext.Provider value={{ user, loading, login, logout, loadUser, isAdmin, isUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be inside AuthProvider');
  return ctx;
}
