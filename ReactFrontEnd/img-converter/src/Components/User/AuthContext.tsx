import React, { createContext, useContext, useEffect, useState } from 'react';
import apiClient, { setAuthToken } from '../API/apiClient';
import { AppUser } from '../../Models/AppUser';

type AuthResponse = {
    token: string;
    email: string;
    username: string;
}

type AuthState = {
    user: AppUser | null;
    token: string | null;
    initializing: boolean;
    login: (email: string, password: string) => Promise<void>;
    register: (email: string, username: string, password: string) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error('useAuth must be used within AuthProvider');
    return ctx;
};

export const AuthProvider: React.FC<{children: React.ReactNode }> = ({children }) => {
    const [user, setUser] = useState<AppUser | null>(null);
    const [token, setToken] = useState<string | null>(null);
    const [initializing, setInitializing] = useState<boolean>(true);

    useEffect(() => {
    const raw = localStorage.getItem('auth');
    if (raw) {
      try {
        const parsed = JSON.parse(raw);
        setToken(parsed.token);
        setUser(parsed.user);
        setAuthToken(parsed.token);
      } catch {
        localStorage.removeItem('auth');
      }
    }
    setInitializing(false);
  }, []);

  const login = async (email: string, password: string) => {
    const resp = await apiClient.post<AuthResponse>('/auth/login', { email, password });
    handleAuthResponse(resp.data);
  };

  const register = async (email: string, username: string, password: string) => {
    const resp = await apiClient.post<AuthResponse>('/auth/register', { email, username, password });
    handleAuthResponse(resp.data);
  };

  const handleAuthResponse = (data: AuthResponse) => {
    const { token, email, username } = data;
    const userObj: AppUser = { email, username };

    localStorage.setItem('auth', JSON.stringify({ token, user: userObj }));
    setToken(token);
    setUser(userObj);
    setAuthToken(token);
  };
    
    const logout = () => {
    localStorage.removeItem('auth');
    setToken(null);
    setUser(null);
    setAuthToken(undefined);
  };

  return (
    <AuthContext.Provider value={{ user, token, initializing, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};