import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

import Layout from '@/components/Layout';
import Home from '@/pages/Home';
import ProductDetails from '@/pages/ProductDetails';
import Cart from '@/pages/Cart';
import Login from '@/pages/Login';
import Register from '@/pages/Register';
import ForgotPassword from '@/pages/ForgotPassword';
import ResetPassword from '@/pages/ResetPassword';
import MyOrders from '@/pages/MyOrders';
import OrderDetails from '@/pages/OrderDetails';
import AdminProducts from '@/pages/Products';
import AdminOrders from '@/pages/Orders';
import { AuthProvider, useAuth } from '@/contexts/AuthContext';

const AppRoutes = () => {
  const { isAuthenticated, isAdmin } = useAuth();

  const PrivateRoute = ({
    children,
    adminOnly = false,
  }: {
    children: React.ReactElement;
    adminOnly?: boolean;
  }) => {
    if (!isAuthenticated) {
      return <Navigate to="/login" replace />;
    }

    if (adminOnly && !isAdmin) {
      return <Navigate to="/" replace />;
    }

    return children;
  };

  const renderWithLayout = (node: React.ReactElement) => <Layout>{node}</Layout>;

  return (
    <Routes>
      <Route path="/" element={renderWithLayout(<Home />)} />
      <Route path="/products/:id" element={renderWithLayout(<ProductDetails />)} />

      <Route
        path="/cart"
        element={
          <PrivateRoute>
            {renderWithLayout(<Cart />)}
          </PrivateRoute>
        }
      />
      <Route
        path="/my-orders"
        element={
          <PrivateRoute>
            {renderWithLayout(<MyOrders />)}
          </PrivateRoute>
        }
      />
      <Route
        path="/orders/:id"
        element={
          <PrivateRoute>
            {renderWithLayout(<OrderDetails />)}
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/products"
        element={
          <PrivateRoute adminOnly>
            {renderWithLayout(<AdminProducts />)}
          </PrivateRoute>
        }
      />
      <Route
        path="/admin/orders"
        element={
          <PrivateRoute adminOnly>
            {renderWithLayout(<AdminOrders />)}
          </PrivateRoute>
        }
      />

      <Route path="/login" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/reset-password" element={<ResetPassword />} />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ToastContainer position="top-right" theme="colored" />
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
