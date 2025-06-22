export interface ProductVariant {
  id: string;
  title: string;
  sku: string;
  price: number;
  compareAtPrice?: number;
  inventoryQuantity: number;
  weight?: number;
  weightUnit?: string;
  option1?: string;
  option2?: string;
  option3?: string;
  imageUrl?: string;
  isAvailable: boolean;
}

export interface ProductImage {
  id: string;
  url: string;
  altText?: string;
  position: number;
  width: number;
  height: number;
}

export interface ProductOption {
  id: string;
  name: string;
  position: number;
  values: string[];
}

export interface Product {
  id: string;
  title: string;
  description: string;
  vendor: string;
  productType: string;
  tags: string[];
  variants: ProductVariant[];
  images: ProductImage[];
  options: ProductOption[];
  status: 'active' | 'draft' | 'archived';
  createdAt: string;
  updatedAt: string;
  publishedAt?: string;
  seoTitle?: string;
  seoDescription?: string;
  handle: string;
}

export interface ProductFilters {
  search?: string;
  category?: string;
  vendor?: string;
  priceMin?: number;
  priceMax?: number;
  inStock?: boolean;
  tags?: string[];
  status?: 'active' | 'draft' | 'archived';
}

export interface ProductSortOptions {
  field: 'title' | 'price' | 'createdAt' | 'updatedAt' | 'popularity';
  direction: 'asc' | 'desc';
}

export interface ProductPagination {
  page: number;
  limit: number;
}

export interface ProductSearchResult {
  products: Product[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

export interface CreateProductRequest {
  title: string;
  description: string;
  vendor: string;
  productType: string;
  tags: string[];
  status: 'active' | 'draft' | 'archived';
  seoTitle?: string;
  seoDescription?: string;
  handle: string;
  variants?: any[];
  images?: any[];
  options?: any[];
  published?: boolean;
}

export interface UpdateProductRequest {
  title?: string;
  description?: string;
  vendor?: string;
  productType?: string;
  tags?: string[];
  status?: 'active' | 'draft' | 'archived';
  seoTitle?: string;
  seoDescription?: string;
} 