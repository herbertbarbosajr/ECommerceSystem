import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

interface LayoutProps {
  children: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const { isAuthenticated, isAdmin, user, logout } = useAuth();

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Navigation */}
      <nav className="bg-white shadow-lg">
        <div className="max-w-7xl mx-auto px-4">
          <div className="flex justify-between h-16">
            <div className="flex">
              {/* Logo */}
              <Link to="/" className="flex items-center">
                <span className="text-xl font-bold text-gray-800">ECommerce</span>
              </Link>

              {/* Navigation Links */}
              <div className="ml-10 flex items-center space-x-4">
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
            <div className="flex items-center">
              {isAuthenticated ? (
                <div className="flex items-center space-x-4">
                  <Link to="/cart" className="text-gray-600 hover:text-gray-900">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </Link>
                  <span className="text-gray-600">{user?.email}</span>
                  <button
                    onClick={logout}
                    className="text-gray-600 hover:text-gray-900"
                  >
                    Sair
                  </button>
                </div>
              ) : (
                <div className="flex items-center space-x-4">
                  <Link
                    to="/login"
                    className="text-gray-600 hover:text-gray-900"
                  >
                    Login
                  </Link>
                  <Link
                    to="/register"
                    className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600"
                  >
                    Registrar
                  </Link>
                </div>
              )}
            </div>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
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