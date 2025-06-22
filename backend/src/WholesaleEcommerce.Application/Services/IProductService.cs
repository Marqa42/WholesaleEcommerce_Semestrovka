using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Application.Services;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, User? currentUser = null);
    Task<ProductDto?> GetByHandleAsync(string handle, User? currentUser = null);
    Task<ProductSearchResponse> SearchAsync(ProductSearchRequest request, User? currentUser = null);
    Task<List<ProductDto>> GetByCategoryAsync(string category, int page = 1, int pageSize = 20, User? currentUser = null);
    Task<List<ProductDto>> GetByVendorAsync(string vendor, int page = 1, int pageSize = 20, User? currentUser = null);
    Task<List<ProductDto>> GetFeaturedAsync(int limit = 10, User? currentUser = null);
    Task<List<ProductDto>> GetRelatedAsync(Guid productId, int limit = 5, User? currentUser = null);
    
    Task<ProductDto> CreateAsync(CreateProductRequest request, User currentUser);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, User currentUser);
    Task DeleteAsync(Guid id, User currentUser);
    
    Task UpdateInventoryAsync(Guid productId, string sku, int quantity, User currentUser);
    Task<int> GetProductCountAsync(ProductSearchRequest? request = null, User? currentUser = null);
    Task<int> GetCountAsync(ProductSearchRequest? request = null, User? currentUser = null);
} 