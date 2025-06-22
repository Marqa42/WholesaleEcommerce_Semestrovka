import apiService from './api';
import {
  Product,
  ProductFilters,
  ProductSortOptions,
  ProductPagination,
  ProductSearchResult,
  CreateProductRequest,
  UpdateProductRequest
} from '@/types/product';

export class ProductService {
  // Получить продукт по ID
  async getProduct(id: string): Promise<Product> {
    return apiService.get<Product>(`/products/${id}`);
  }

  // Получить продукт по handle
  async getProductByHandle(handle: string): Promise<Product> {
    return apiService.get<Product>(`/products/handle/${handle}`);
  }

  // Поиск продуктов
  async searchProducts(
    filters: ProductFilters,
    sort: ProductSortOptions,
    pagination: ProductPagination
  ): Promise<ProductSearchResult> {
    const params = new URLSearchParams();
    
    if (filters.search) params.append('search', filters.search);
    if (filters.category) params.append('category', filters.category);
    if (filters.vendor) params.append('vendor', filters.vendor);
    if (filters.priceMin) params.append('priceMin', filters.priceMin.toString());
    if (filters.priceMax) params.append('priceMax', filters.priceMax.toString());
    if (filters.inStock) params.append('inStock', filters.inStock.toString());
    if (filters.tags) params.append('tags', filters.tags.join(','));
    if (filters.status) params.append('status', filters.status);
    
    params.append('sortField', sort.field);
    params.append('sortDirection', sort.direction);
    params.append('page', pagination.page.toString());
    params.append('limit', pagination.limit.toString());

    return apiService.get<ProductSearchResult>(`/products/search?${params.toString()}`);
  }

  // Получить продукты по категории
  async getProductsByCategory(
    category: string,
    pagination: ProductPagination
  ): Promise<ProductSearchResult> {
    const params = new URLSearchParams();
    params.append('page', pagination.page.toString());
    params.append('limit', pagination.limit.toString());

    return apiService.get<ProductSearchResult>(`/products/category/${category}?${params.toString()}`);
  }

  // Получить продукты по вендору
  async getProductsByVendor(
    vendor: string,
    pagination: ProductPagination
  ): Promise<ProductSearchResult> {
    const params = new URLSearchParams();
    params.append('page', pagination.page.toString());
    params.append('limit', pagination.limit.toString());

    return apiService.get<ProductSearchResult>(`/products/vendor/${vendor}?${params.toString()}`);
  }

  // Получить избранные продукты
  async getFeaturedProducts(limit: number = 10): Promise<Product[]> {
    return apiService.get<Product[]>(`/products/featured?limit=${limit}`);
  }

  // Получить связанные продукты
  async getRelatedProducts(productId: string, limit: number = 5): Promise<Product[]> {
    return apiService.get<Product[]>(`/products/${productId}/related?limit=${limit}`);
  }

  // Создать продукт (только для админов)
  async createProduct(data: CreateProductRequest): Promise<Product> {
    return apiService.post<Product>('/products', data);
  }

  // Обновить продукт (только для админов)
  async updateProduct(id: string, data: UpdateProductRequest): Promise<Product> {
    return apiService.put<Product>(`/products/${id}`, data);
  }

  // Удалить продукт (только для админов)
  async deleteProduct(id: string): Promise<void> {
    return apiService.delete<void>(`/products/${id}`);
  }

  // Обновить инвентарь (только для админов)
  async updateInventory(productId: string, variantSku: string, quantity: number): Promise<void> {
    return apiService.patch<void>(`/products/${productId}/inventory/${variantSku}`, { quantity });
  }

  // Получить количество продуктов
  async getProductCount(filters?: ProductFilters): Promise<number> {
    const params = new URLSearchParams();
    
    if (filters?.search) params.append('search', filters.search);
    if (filters?.category) params.append('category', filters.category);
    if (filters?.vendor) params.append('vendor', filters.vendor);
    if (filters?.status) params.append('status', filters.status);

    return apiService.get<{ count: number }>(`/products/count?${params.toString()}`).then(res => res.count);
  }
}

export const productService = new ProductService();
export default productService; 