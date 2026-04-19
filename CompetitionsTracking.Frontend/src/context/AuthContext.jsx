import React, { createContext, useState, useEffect, useContext } from 'react';
import { jwtDecode } from 'jwt-decode';
import api from '../utils/axiosSetup';

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const initAuth = () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const decoded = jwtDecode(token);
          if (decoded.exp * 1000 < Date.now()) {
            logout();
          } else {
            const roleKey = Object.keys(decoded).find(k => k.endsWith('/claims/role')) || 'role';
            const usernameKey = Object.keys(decoded).find(k => k.endsWith('/claims/name')) || 'unique_name';
            
            setUser({
              username: decoded[usernameKey],
              role: decoded[roleKey],
              token: token
            });
          }
        } catch (e) {
          console.error("Invalid token", e);
          logout();
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (credentials) => {
    try {
      const response = await api.post('/Auth/login', credentials);
      const { token } = response.data;
      if (token) {
        localStorage.setItem('token', token);
        const decoded = jwtDecode(token);
        const roleKey = Object.keys(decoded).find(k => k.endsWith('/claims/role')) || 'role';
        const usernameKey = Object.keys(decoded).find(k => k.endsWith('/claims/name')) || 'unique_name';
        setUser({
          username: decoded[usernameKey],
          role: decoded[roleKey],
          token: token
        });
        return { success: true };
      }
      return { success: false, message: "No token received" };
    } catch (error) {
       return { success: false, message: error.message || "Login failed" };
    }
  };

  const register = async (userData) => {
      try {
          await api.post('/Auth/register', userData);
          return { success: true };
      } catch (error) {
          return { success: false, message: error.message || "Registration failed", validationErrors: error.validationErrors };
      }
  }

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const isAdmin = user?.role === 'Admin';

  const value = {
    user,
    login,
    register,
    logout,
    loading,
    isAdmin
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};
