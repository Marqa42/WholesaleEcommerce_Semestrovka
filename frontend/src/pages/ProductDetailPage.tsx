import React from 'react';
import { useParams } from 'react-router-dom';
import { useQuery } from 'react-query';
import { productService } from '@/services/productService';

const ProductDetailPage: React.FC = () => {
  const { handle } = useParams<{ handle: string }>();

  const { data: product, isLoading, error } = useQuery(
    ['product', handle],
    () => productService.getProductByHandle(handle!),
    {
      enabled: !!handle,
    }
  );

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error || !product) {
    return (
      <div className="text-center py-8">
        <p className="text-red-600">Product not found.</p>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">{product.title}</h1>
          <p className="text-lg text-gray-600 mt-2">{product.vendor}</p>
        </div>
        <div>
          <p className="text-gray-700">{product.description}</p>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailPage; 