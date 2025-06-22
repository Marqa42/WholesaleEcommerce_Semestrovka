import React from 'react';
import { Routes, Route, Link } from 'react-router-dom';

const AdminPage: React.FC = () => {
  return (
    <div className="space-y-6">
      <div className="bg-white shadow rounded-lg p-6">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Admin Dashboard</h1>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="bg-blue-50 p-6 rounded-lg">
            <h3 className="text-lg font-semibold text-blue-900 mb-2">Products</h3>
            <p className="text-blue-700 mb-4">Manage your product catalog</p>
            <Link
              to="/admin/products"
              className="bg-blue-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-blue-700"
            >
              Manage Products
            </Link>
          </div>

          <div className="bg-green-50 p-6 rounded-lg">
            <h3 className="text-lg font-semibold text-green-900 mb-2">Users</h3>
            <p className="text-green-700 mb-4">Manage user accounts</p>
            <Link
              to="/admin/users"
              className="bg-green-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-green-700"
            >
              Manage Users
            </Link>
          </div>

          <div className="bg-purple-50 p-6 rounded-lg">
            <h3 className="text-lg font-semibold text-purple-900 mb-2">Orders</h3>
            <p className="text-purple-700 mb-4">View and manage orders</p>
            <Link
              to="/admin/orders"
              className="bg-purple-600 text-white px-4 py-2 rounded-md text-sm font-medium hover:bg-purple-700"
            >
              View Orders
            </Link>
          </div>
        </div>
      </div>

      <Routes>
        <Route path="/" element={<AdminDashboard />} />
        <Route path="/products" element={<AdminProducts />} />
        <Route path="/users" element={<AdminUsers />} />
        <Route path="/orders" element={<AdminOrders />} />
      </Routes>
    </div>
  );
};

const AdminDashboard: React.FC = () => (
  <div className="bg-white shadow rounded-lg p-6">
    <h2 className="text-xl font-semibold mb-4">Dashboard Overview</h2>
    <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div className="bg-blue-100 p-4 rounded-lg">
        <h3 className="font-semibold text-blue-900">Total Products</h3>
        <p className="text-2xl font-bold text-blue-700">150</p>
      </div>
      <div className="bg-green-100 p-4 rounded-lg">
        <h3 className="font-semibold text-green-900">Total Users</h3>
        <p className="text-2xl font-bold text-green-700">1,250</p>
      </div>
      <div className="bg-yellow-100 p-4 rounded-lg">
        <h3 className="font-semibold text-yellow-900">Total Orders</h3>
        <p className="text-2xl font-bold text-yellow-700">3,420</p>
      </div>
      <div className="bg-purple-100 p-4 rounded-lg">
        <h3 className="font-semibold text-purple-900">Revenue</h3>
        <p className="text-2xl font-bold text-purple-700">$45,230</p>
      </div>
    </div>
  </div>
);

const AdminProducts: React.FC = () => (
  <div className="bg-white shadow rounded-lg p-6">
    <h2 className="text-xl font-semibold mb-4">Product Management</h2>
    <p className="text-gray-600">Product management interface will be implemented here.</p>
  </div>
);

const AdminUsers: React.FC = () => (
  <div className="bg-white shadow rounded-lg p-6">
    <h2 className="text-xl font-semibold mb-4">User Management</h2>
    <p className="text-gray-600">User management interface will be implemented here.</p>
  </div>
);

const AdminOrders: React.FC = () => (
  <div className="bg-white shadow rounded-lg p-6">
    <h2 className="text-xl font-semibold mb-4">Order Management</h2>
    <p className="text-gray-600">Order management interface will be implemented here.</p>
  </div>
);

export default AdminPage; 