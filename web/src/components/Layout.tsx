import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTheme } from './ThemeProvider';
import { Moon, Sun } from 'lucide-react';
import { Button } from './ui/button';

interface LayoutProps {
  children: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const [isMobileMenuOpen, setMobileMenuOpen] = useState(false);
  const { isAuthenticated, isAdmin, user, logout } = useAuth();
  const { theme, setTheme } = useTheme();

  return (
    <div className="min-h-screen bg-background text-foreground flex flex-col">
      {/* Navigation */}
      <nav className="bg-card border-b shadow-sm">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center">
              {/* Logo */}
              <Link to="/" className="flex items-center animate-fade-in">
                <span className="text-xl font-bold bg-gradient-to-r from-primary to-primary/80 bg-clip-text text-transparent">ECommerce</span>
              </Link>

              {/* Desktop Navigation Links */}
              <div className="hidden md:ml-10 md:flex md:items-center md:space-x-4">
                {isAuthenticated && (
                  <>
                    <Link to="/" className="text-gray-600 hover:text-gray-900">
                      Produtos
                    </Link>
                    <Link to="/orders" className="text-gray-600 hover:text-gray-900">
                      Meus Pedidos
                    </Link>
                  </>
                )}

                {isAdmin && (
                  <>
                    <Link to="/admin/products" className="text-gray-600 hover:text-gray-900">
                      Gerenciar Produtos
                    </Link>
                    <Link to="/admin/orders" className="text-gray-600 hover:text-gray-900">
                      Gerenciar Pedidos
                    </Link>
                  </>
                )}
              </div>
            </div>

            {/* Right side */}
            <div className="hidden md:flex md:items-center space-x-4">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setTheme(theme === "light" ? "dark" : "light")}
                className="w-9 px-0"
              >
                <Sun className="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
                <Moon className="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
                <span className="sr-only">Toggle theme</span>
              </Button>
              {isAuthenticated ? (
                <div className="flex items-center space-x-4">
                  <Link to="/cart" className="text-muted-foreground hover:text-foreground transition-colors duration-200">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </Link>
                  <span className="text-muted-foreground">{user?.email}</span>
                  <button
                    onClick={logout}
                    className="text-muted-foreground hover:text-foreground transition-colors duration-200"
                  >
                    Sair
                  </button>
                </div>
              ) : (
                <div className="flex items-center space-x-4">
                  <Link
                    to="/login"
                    className="text-muted-foreground hover:text-foreground transition-colors duration-200"
                  >
                    Login
                  </Link>
                  <Link
                    to="/register"
                    className="bg-primary text-primary-foreground px-4 py-2 rounded-lg hover:bg-primary/90 transition-all duration-200 transform hover:scale-105"
                  >
                    Registrar
                  </Link>
                </div>
              )}
            </div>

            {/* Mobile Menu Button */}
            <div className="md:hidden flex items-center space-x-2">
              <Button
                variant="ghost"
                size="sm"
                onClick={() => setTheme(theme === "light" ? "dark" : "light")}
                className="w-9 px-0"
              >
                <Sun className="h-[1.2rem] w-[1.2rem] rotate-0 scale-100 transition-all dark:-rotate-90 dark:scale-0" />
                <Moon className="absolute h-[1.2rem] w-[1.2rem] rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
                <span className="sr-only">Toggle theme</span>
              </Button>
              <button
                onClick={() => setMobileMenuOpen(!isMobileMenuOpen)}
                className="inline-flex items-center justify-center p-2 rounded-lg text-slate-400 hover:text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-blue-500 transition-colors duration-200"
              >
                <span className="sr-only">Abrir menu principal</span>
                {/* Icon for menu (hamburger) */}
                <svg className="h-6 w-6" stroke="currentColor" fill="none" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16m-7 6h7" />
                </svg>
              </button>
            </div>
          </div>
        </div>

        {/* Mobile Menu, show/hide based on state */}
        {isMobileMenuOpen && (
          <div className="md:hidden">
            <div className="px-2 pt-2 pb-3 space-y-1 sm:px-3">
              {isAuthenticated && (
                <>
                  <Link to="/" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Produtos</Link>
                  <Link to="/orders" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Meus Pedidos</Link>
                </>
              )}
              {isAdmin && (
                <>
                  <Link to="/admin/products" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Gerenciar Produtos</Link>
                  <Link to="/admin/orders" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Gerenciar Pedidos</Link>
                </>
              )}
            </div>
            <div className="pt-4 pb-3 border-t border-gray-200">
              {isAuthenticated ? (
                <div className="px-2 space-y-1">
                  <div className="px-3 py-2">
                    <div className="text-base font-medium text-gray-800">{user?.email}</div>
                  </div>
                  <Link to="/cart" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Carrinho</Link>
                  <button onClick={logout} className="w-full text-left text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">
                    Sair
                  </button>
                </div>
              ) : (
                <div className="px-2 space-y-1">
                  <Link to="/login" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Login</Link>
                  <Link to="/register" className="text-gray-600 hover:bg-gray-50 hover:text-gray-900 block px-3 py-2 rounded-md text-base font-medium">Registrar</Link>
                </div>
              )}
            </div>
          </div>
        )}
      </nav>

      {/* Main Content */}
      <main className="flex-grow max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8 w-full">
        {children}
      </main>

      {/* Footer */}
      <footer className="bg-white shadow-lg mt-8">
        <div className="max-w-7xl mx-auto py-6 px-4">
          <div className="text-center text-gray-500">
            © 2025 ECommerce. Todos os direitos reservados.
          </div>
        </div>
      </footer>
    </div>
  );
}