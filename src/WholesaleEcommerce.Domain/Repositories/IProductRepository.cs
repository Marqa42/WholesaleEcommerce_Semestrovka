using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetByHandleAsync(string handle);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByHandleAsync(string handle);
    
    // Search and filtering
    Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        ProductSearchCriteria criteria,
        int page = 1,
        int pageSize = 20);
    
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, int page = 1, int pageSize = 20);
    Task<IEnumerable<Product>> GetByVendorAsync(string vendor, int page = 1, int pageSize = 20);
    Task<IEnumerable<Product>> GetFeaturedAsync(int limit = 10);
    Task<IEnumerable<Product>> GetRelatedAsync(Guid productId, int limit = 5);
    
    // Inventory management
    Task UpdateInventoryAsync(Guid productId, string sku, int quantity);
    Task<int> GetInventoryAsync(Guid productId, string sku);
    
    // Counts
    Task<int> GetCountAsync(ProductSearchCriteria? criteria = null);
} 