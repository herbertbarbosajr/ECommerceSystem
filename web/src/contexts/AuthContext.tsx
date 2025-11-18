import React, { createContext, useState, useContext, useEffect, ReactNode } from 'react';
import { jwtDecode } from 'jwt-decode';
import apiClient, { auth } from '@/services/api'; // Importando as funções de auth

// Estrutura do payload do token JWT
interface DecodedToken {
  sub: string; // Subject (user ID)
  email: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string; // Role claim
  role?: string; // Alternative role claim
  IsAdmin?: string; // Custom claim
  exp: number;
}

// Estrutura do usuário
interface User {
  id: string;
  email: string;
  roles: string[];
}

// Tipos para o contexto de autenticação
export interface AuthContextType {
  isAuthenticated: boolean;
  isAdmin: boolean;
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  register: (firstName: string, lastName: string, email: string, password: string) => Promise<void>; // Adicionando a função register
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded = jwtDecode<DecodedToken>(token);
        if (decoded.exp * 1000 > Date.now()) {
          // Extract role from token (can be in different claim formats)
          const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || '';
          const isAdmin = decoded.IsAdmin === 'True' || role === 'Admin';
          const roles = role ? [role] : [];
          
          setUser({ id: decoded.sub, email: decoded.email, roles });
          apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        } else {
          localStorage.removeItem('token');
        }
      } catch (error) {
        console.error('Invalid token:', error);
        localStorage.removeItem('token');
      }
    }
  }, []);

  const login = async (email: string, password: string) => {
    const response = await auth.login(email, password);
    // API retorna em camelCase devido à serialização JSON
    const token = response.token;
    if (!token) {
      throw new Error('Token não recebido da API');
    }
    localStorage.setItem('token', token);
    const decoded = jwtDecode<DecodedToken>(token);

    // Extract role from token (can be in different claim formats)
    const role = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role || '';
    const isAdmin = decoded.IsAdmin === 'True' || role === 'Admin';
    const roles = role ? [role] : [];

    setUser({ id: decoded.sub, email: decoded.email, roles });
    apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  };

  // Implementação da função de registro
  const register = async (firstName: string, lastName: string,email: string, password: string) => {
    await auth.register({ firstName, lastName,email, password });
    // Após o registro, não fazemos login automático, o usuário será redirecionado para a tela de login.
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('token');
    delete apiClient.defaults.headers.common['Authorization'];
  };

  const value: AuthContextType = {
    isAuthenticated: !!user,
    isAdmin: user?.roles.includes('Admin') ?? false,
    user,
    login,
    logout,
    register, // Expondo a função no provedor
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};