import React from 'react';
import { Link } from 'react-router-dom';
import { Product } from '@/types/product';

interface ProductCardProps {
  product: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  const mainImage = product.images.find(img => img.position === 1) || product.images[0];
  const minPrice = Math.min(...product.variants.map(v => v.price));
  const maxPrice = Math.max(...product.variants.map(v => v.price));
  const isAvailable = product.variants.some(v => v.isAvailable);

  return (
    <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
      <Link to={`/products/${product.handle}`}>
        <div className="aspect-w-1 aspect-h-1 w-full">
          {mainImage ? (
            <img
              src={mainImage.url}
              alt={mainImage.altText || product.title}
              className="w-full h-48 object-cover"
            />
          ) : (
            <div className="w-full h-48 bg-gray-200 flex items-center justify-center">
              <span className="text-gray-400">No image</span>
            </div>
          )}
        </div>
      </Link>

      <div className="p-4">
        <Link to={`/products/${product.handle}`}>
          <h3 className="text-lg font-semibold text-gray-900 mb-2 hover:text-blue-600 transition-colors">
            {product.title}
          </h3>
        </Link>

        <p className="text-sm text-gray-600 mb-2 line-clamp-2">
          {product.description}
        </p>

        <div className="flex items-center justify-between mb-2">
          <span className="text-sm text-gray-500">Vendor: {product.vendor}</span>
          <span className="text-sm text-gray-500">{product.productType}</span>
        </div>

        <div className="flex items-center justify-between">
          <div className="text-lg font-bold text-gray-900">
            {minPrice === maxPrice ? (
              `$${minPrice.toFixed(2)}`
            ) : (
              `$${minPrice.toFixed(2)} - $${maxPrice.toFixed(2)}`
            )}
          </div>
          
          <div className="flex items-center space-x-2">
            {isAvailable ? (
              <span className="text-green-600 text-sm font-medium">In Stock</span>
            ) : (
              <span className="text-red-600 text-sm font-medium">Out of Stock</span>
            )}
          </div>
        </div>

        {product.tags.length > 0 && (
          <div className="mt-3 flex flex-wrap gap-1">
            {product.tags.slice(0, 3).map((tag) => (
              <span
                key={tag}
                className="inline-block bg-gray-100 text-gray-700 text-xs px-2 py-1 rounded"
              >
                {tag}
              </span>
            ))}
            {product.tags.length > 3 && (
              <span className="text-xs text-gray-500">+{product.tags.length - 3} more</span>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProductCard; 