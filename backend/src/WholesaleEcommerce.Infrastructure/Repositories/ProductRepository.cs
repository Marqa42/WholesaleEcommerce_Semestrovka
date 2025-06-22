using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;
using WholesaleEcommerce.Infrastructure.Data;

namespace WholesaleEcommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByHandleAsync(string handle)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Handle == handle);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .ToListAsync();
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByHandleAsync(string handle)
    {
        return await _context.Products.AnyAsync(p => p.Handle == handle);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        ProductSearchCriteria criteria,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(criteria.Search))
        {
            var searchTerm = criteria.Search.ToLower();
            query = query.Where(p => 
                p.Title.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                p.Vendor.ToLower().Contains(searchTerm) ||
                p.ProductType.ToLower().Contains(searchTerm) ||
                p.Tags.Any(tag => tag.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrEmpty(criteria.Category))
        {
            query = query.Where(p => p.ProductType.ToLower() == criteria.Category.ToLower());
        }

        if (!string.IsNullOrEmpty(criteria.Vendor))
        {
            query = query.Where(p => p.Vendor.ToLower() == criteria.Vendor.ToLower());
        }

        if (criteria.MinPrice.HasValue)
        {
            query = query.Where(p => p.Variants.Any(v => v.Price >= criteria.MinPrice.Value));
        }

        if (criteria.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Variants.Any(v => v.Price <= criteria.MaxPrice.Value));
        }

        if (criteria.InStock.HasValue)
        {
            if (criteria.InStock.Value)
            {
                query = query.Where(p => p.Variants.Any(v => v.InventoryQuantity > 0));
            }
            else
            {
                query = query.Where(p => !p.Variants.Any(v => v.InventoryQuantity > 0));
            }
        }

        if (!string.IsNullOrEmpty(criteria.Status))
        {
            query = query.Where(p => p.Status == criteria.Status);
        }

        if (criteria.Tags != null && criteria.Tags.Any())
        {
            query = query.Where(p => criteria.Tags.Any(tag => p.Tags.Contains(tag)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "title" => criteria.SortDescending ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title),
            "price" => criteria.SortDescending ? query.OrderByDescending(p => p.Variants.Min(v => v.Price)) : query.OrderBy(p => p.Variants.Min(v => v.Price)),
            "createdat" => criteria.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "updatedat" => criteria.SortDescending ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),
            _ => criteria.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        // Apply pagination
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, int page = 1, int pageSize = 20)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .Where(p => p.ProductType.ToLower() == category.ToLower())
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByVendorAsync(string vendor, int page = 1, int pageSize = 20)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .Where(p => p.Vendor.ToLower() == vendor.ToLower())
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedAsync(int limit = 10)
    {
        // For now, return the most recent active products
        // In a real application, you might have a Featured flag or algorithm
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .Where(p => p.Status == "active")
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetRelatedAsync(Guid productId, int limit = 5)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return new List<Product>();

        // Get products with the same category or vendor
        return await _context.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .Include(p => p.Options)
            .Where(p => p.Id != productId && 
                       p.Status == "active" &&
                       (p.ProductType == product.ProductType || p.Vendor == product.Vendor))
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task UpdateInventoryAsync(Guid productId, string sku, int quantity)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.ProductId == productId && v.Sku == sku);

        if (variant != null)
        {
            variant.InventoryQuantity = Math.Max(0, variant.InventoryQuantity + quantity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetInventoryAsync(Guid productId, string sku)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.ProductId == productId && v.Sku == sku);

        return variant?.InventoryQuantity ?? 0;
    }

    public async Task<int> GetCountAsync(ProductSearchCriteria? criteria = null)
    {
        var query = _context.Products.AsQueryable();

        if (criteria != null)
        {
            // Apply the same filters as in SearchAsync
            if (!string.IsNullOrEmpty(criteria.Search))
            {
                var searchTerm = criteria.Search.ToLower();
                query = query.Where(p => 
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Description.ToLower().Contains(searchTerm) ||
                    p.Vendor.ToLower().Contains(searchTerm) ||
                    p.ProductType.ToLower().Contains(searchTerm) ||
                    p.Tags.Any(tag => tag.ToLower().Contains(searchTerm))
                );
            }

            if (!string.IsNullOrEmpty(criteria.Category))
            {
                query = query.Where(p => p.ProductType.ToLower() == criteria.Category.ToLower());
            }

            if (!string.IsNullOrEmpty(criteria.Vendor))
            {
                query = query.Where(p => p.Vendor.ToLower() == criteria.Vendor.ToLower());
            }

            if (criteria.MinPrice.HasValue)
            {
                query = query.Where(p => p.Variants.Any(v => v.Price >= criteria.MinPrice.Value));
            }

            if (criteria.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Variants.Any(v => v.Price <= criteria.MaxPrice.Value));
            }

            if (criteria.InStock.HasValue)
            {
                if (criteria.InStock.Value)
                {
                    query = query.Where(p => p.Variants.Any(v => v.InventoryQuantity > 0));
                }
                else
                {
                    query = query.Where(p => !p.Variants.Any(v => v.InventoryQuantity > 0));
                }
            }

            if (!string.IsNullOrEmpty(criteria.Status))
            {
                query = query.Where(p => p.Status == criteria.Status);
            }

            if (criteria.Tags != null && criteria.Tags.Any())
            {
                query = query.Where(p => criteria.Tags.Any(tag => p.Tags.Contains(tag)));
            }
        }

        return await query.CountAsync();
    }
} 