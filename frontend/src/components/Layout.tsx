import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { user, logout } = useAuthStore();
  const location = useLocation();

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            {/* Logo */}
            <Link to="/" className="flex items-center">
              <span className="text-2xl font-bold text-gray-900">Wholesale</span>
            </Link>

            {/* Navigation */}
            <nav className="hidden md:flex space-x-8">
              <Link
                to="/"
                className={`text-sm font-medium ${
                  location.pathname === '/' ? 'text-blue-600' : 'text-gray-500 hover:text-gray-900'
                }`}
              >
                Home
              </Link>
              <Link
                to="/products"
                className={`text-sm font-medium ${
                  location.pathname.startsWith('/products') ? 'text-blue-600' : 'text-gray-500 hover:text-gray-900'
                }`}
              >
                Products
              </Link>
              {user && (
                <Link
                  to="/add-product"
                  className={`text-sm font-medium ${
                    location.pathname === '/add-product' ? 'text-blue-600' : 'text-gray-500 hover:text-gray-900'
                  }`}
                >
                  Add Product
                </Link>
              )}
              {user?.isAdmin && (
                <Link
                  to="/admin"
                  className={`text-sm font-medium ${
                    location.pathname.startsWith('/admin') ? 'text-blue-600' : 'text-gray-500 hover:text-gray-900'
                  }`}
                >
                  Admin
                </Link>
              )}
            </nav>

            {/* User Menu */}
            <div className="flex items-center space-x-4">
              {user ? (
                <div className="flex items-center space-x-4">
                  <span className="text-sm text-gray-700">
                    Welcome, {user.firstName}
                  </span>
                  <Link
                    to="/profile"
                    className="text-sm font-medium text-gray-500 hover:text-gray-900"
                  >
                    Profile
                  </Link>
                  <button
                    onClick={handleLogout}
                    className="text-sm font-medium text-gray-500 hover:text-gray-900"
                  >
                    Logout
                  </button>
                </div>
              ) : (
                <div className="flex items-center space-x-4">
                  <Link
                    to="/login"
                    className="text-sm font-medium text-gray-500 hover:text-gray-900"
                  >
                    Login
                  </Link>
                  <Link
                    to="/register"
                    className="bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-700"
                  >
                    Register
                  </Link>
                </div>
              )}
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {children}
      </main>

      {/* Footer */}
      <footer className="bg-white border-t mt-auto">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            <div>
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Wholesale</h3>
              <p className="text-gray-600">
                Your trusted partner for wholesale products and supplies.
              </p>
            </div>
            <div>
              <h4 className="text-sm font-semibold text-gray-900 mb-4">Products</h4>
              <ul className="space-y-2">
                <li><Link to="/products" className="text-gray-600 hover:text-gray-900">All Products</Link></li>
                <li><Link to="/products?category=electronics" className="text-gray-600 hover:text-gray-900">Electronics</Link></li>
                <li><Link to="/products?category=automotive" className="text-gray-600 hover:text-gray-900">Automotive</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="text-sm font-semibold text-gray-900 mb-4">Support</h4>
              <ul className="space-y-2">
                <li><Link to="/contact" className="text-gray-600 hover:text-gray-900">Contact Us</Link></li>
                <li><Link to="/about" className="text-gray-600 hover:text-gray-900">About Us</Link></li>
              </ul>
            </div>
            <div>
              <h4 className="text-sm font-semibold text-gray-900 mb-4">Legal</h4>
              <ul className="space-y-2">
                <li><Link to="/privacy" className="text-gray-600 hover:text-gray-900">Privacy Policy</Link></li>
                <li><Link to="/terms" className="text-gray-600 hover:text-gray-900">Terms of Service</Link></li>
              </ul>
            </div>
          </div>
          <div className="mt-8 pt-8 border-t border-gray-200">
            <p className="text-center text-gray-600">
              © 2024 Wholesale. All rights reserved.
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default Layout; 