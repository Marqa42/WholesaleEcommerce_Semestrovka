using Microsoft.Extensions.Logging;
using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;

namespace WholesaleEcommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, User? currentUser = null)
    {
        var product = await _productRepository.GetByIdAsync(id);
        
        if (product == null)
            return null;

        // If product is not active, show only to admins
        if (product.Status != "active" && (currentUser == null || !currentUser.IsAdmin))
            return null;

        return MapToDto(product);
    }

    public async Task<ProductDto?> GetByHandleAsync(string handle, User? currentUser = null)
    {
        var product = await _productRepository.GetByHandleAsync(handle);
        
        if (product == null)
            return null;

        // If product is not active, show only to admins
        if (product.Status != "active" && (currentUser == null || !currentUser.IsAdmin))
            return null;

        return MapToDto(product);
    }

    public async Task<ProductSearchResponse> SearchAsync(ProductSearchRequest request, User? currentUser = null)
    {
        var criteria = new ProductSearchCriteria
        {
            Search = request.Search,
            Category = request.Category,
            Vendor = request.Vendor,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            InStock = request.InStock,
            Status = request.Status,
            Tags = request.Tags,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        // If user is not admin, show only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            criteria.Status = "active";
        }

        var (products, totalCount) = await _productRepository.SearchAsync(criteria, request.Page, request.PageSize);
        
        var productDtos = products.Select(MapToDto).ToList();
        
        return new ProductSearchResponse
        {
            Products = productDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<List<ProductDto>> GetByCategoryAsync(string category, int page = 1, int pageSize = 20, User? currentUser = null)
    {
        var products = await _productRepository.GetByCategoryAsync(category, page, pageSize);
        
        // If user is not admin, filter only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            products = products.Where(p => p.Status == "active").ToList();
        }

        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetByVendorAsync(string vendor, int page = 1, int pageSize = 20, User? currentUser = null)
    {
        var products = await _productRepository.GetByVendorAsync(vendor, page, pageSize);
        
        // If user is not admin, filter only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            products = products.Where(p => p.Status == "active").ToList();
        }

        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetFeaturedAsync(int limit = 10, User? currentUser = null)
    {
        var products = await _productRepository.GetFeaturedAsync(limit);
        
        // If user is not admin, filter only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            products = products.Where(p => p.Status == "active").ToList();
        }

        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetRelatedAsync(Guid productId, int limit = 5, User? currentUser = null)
    {
        var products = await _productRepository.GetRelatedAsync(productId, limit);
        
        // If user is not admin, filter only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            products = products.Where(p => p.Status == "active").ToList();
        }

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, User currentUser)
    {
        // Only admins can create products
        if (!currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can create products");
        }

        // Check if handle already exists
        if (await _productRepository.ExistsByHandleAsync(request.Handle))
        {
            throw new InvalidOperationException($"Product with handle '{request.Handle}' already exists");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Vendor = request.Vendor,
            ProductType = request.ProductType,
            Tags = request.Tags,
            Status = request.Status,
            Handle = request.Handle,
            SeoTitle = request.SeoTitle,
            SeoDescription = request.SeoDescription,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add variants
        foreach (var variantRequest in request.Variants)
        {
            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                Title = variantRequest.Title,
                Sku = variantRequest.Sku,
                Price = variantRequest.Price,
                CompareAtPrice = variantRequest.CompareAtPrice,
                InventoryQuantity = variantRequest.InventoryQuantity,
                Weight = variantRequest.Weight,
                WeightUnit = variantRequest.WeightUnit,
                Option1 = variantRequest.Option1,
                Option2 = variantRequest.Option2,
                Option3 = variantRequest.Option3,
                ImageUrl = variantRequest.ImageUrl,
                ProductId = product.Id
            };
            product.Variants.Add(variant);
        }

        // Add images
        foreach (var imageRequest in request.Images)
        {
            var image = new ProductImage
            {
                Id = Guid.NewGuid(),
                Url = imageRequest.Url,
                AltText = imageRequest.AltText,
                Position = imageRequest.Position,
                Width = imageRequest.Width,
                Height = imageRequest.Height,
                ProductId = product.Id
            };
            product.Images.Add(image);
        }

        // Add options
        foreach (var optionRequest in request.Options)
        {
            var option = new ProductOption
            {
                Id = Guid.NewGuid(),
                Name = optionRequest.Name,
                Position = optionRequest.Position,
                Values = optionRequest.Values,
                ProductId = product.Id
            };
            product.Options.Add(option);
        }

        var createdProduct = await _productRepository.AddAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, User currentUser)
    {
        // Only admins can update products
        if (!currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can update products");
        }

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        // Update properties if provided
        if (!string.IsNullOrEmpty(request.Title))
            existingProduct.Title = request.Title;
        
        if (!string.IsNullOrEmpty(request.Description))
            existingProduct.Description = request.Description;
        
        if (!string.IsNullOrEmpty(request.Vendor))
            existingProduct.Vendor = request.Vendor;
        
        if (!string.IsNullOrEmpty(request.ProductType))
            existingProduct.ProductType = request.ProductType;
        
        if (request.Tags != null)
            existingProduct.Tags = request.Tags;
        
        if (!string.IsNullOrEmpty(request.Status))
            existingProduct.Status = request.Status;
        
        if (request.SeoTitle != null)
            existingProduct.SeoTitle = request.SeoTitle;
        
        if (request.SeoDescription != null)
            existingProduct.SeoDescription = request.SeoDescription;

        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        return MapToDto(updatedProduct);
    }

    public async Task DeleteAsync(Guid id, User currentUser)
    {
        // Only admins can delete products
        if (!currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can delete products");
        }

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        await _productRepository.DeleteAsync(id);
    }

    public async Task UpdateInventoryAsync(Guid productId, string sku, int quantity, User currentUser)
    {
        // Only admins can update inventory
        if (!currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can update inventory");
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }

        if (!product.HasVariant(sku))
        {
            throw new InvalidOperationException("Product variant not found");
        }

        await _productRepository.UpdateInventoryAsync(productId, sku, quantity);
    }

    public async Task<int> GetProductCountAsync(ProductSearchRequest? request = null, User? currentUser = null)
    {
        ProductSearchCriteria? criteria = null;
        
        if (request != null)
        {
            criteria = new ProductSearchCriteria
            {
                Search = request.Search,
                Category = request.Category,
                Vendor = request.Vendor,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                InStock = request.InStock,
                Status = request.Status,
                Tags = request.Tags
            };
        }

        // If user is not admin, count only active products
        if (currentUser == null || !currentUser.IsAdmin)
        {
            criteria ??= new ProductSearchCriteria();
            criteria.Status = "active";
        }

        return await _productRepository.GetCountAsync(criteria);
    }

    public async Task<int> GetCountAsync(ProductSearchRequest? request = null, User? currentUser = null)
    {
        ProductSearchCriteria? criteria = null;
        if (request != null)
        {
            criteria = new ProductSearchCriteria
            {
                Search = request.Search,
                Category = request.Category,
                Vendor = request.Vendor,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice,
                InStock = request.InStock,
                Status = request.Status,
                Tags = request.Tags
            };
        }
        // Если не админ, считаем только активные товары
        if (currentUser == null || !currentUser.IsAdmin)
        {
            criteria ??= new ProductSearchCriteria();
            criteria.Status = "active";
        }
        return await _productRepository.GetCountAsync(criteria);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Vendor = product.Vendor,
            ProductType = product.ProductType,
            Tags = product.Tags,
            Variants = product.Variants.Select(v => new ProductVariantDto
            {
                Id = v.Id,
                Title = v.Title,
                Sku = v.Sku,
                Price = v.Price,
                CompareAtPrice = v.CompareAtPrice,
                InventoryQuantity = v.InventoryQuantity,
                Weight = v.Weight,
                WeightUnit = v.WeightUnit,
                Option1 = v.Option1,
                Option2 = v.Option2,
                Option3 = v.Option3,
                ImageUrl = v.ImageUrl,
                IsAvailable = v.IsAvailable
            }).ToList(),
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                Url = i.Url,
                AltText = i.AltText,
                Position = i.Position,
                Width = i.Width,
                Height = i.Height
            }).ToList(),
            Options = product.Options.Select(o => new ProductOptionDto
            {
                Id = o.Id,
                Name = o.Name,
                Position = o.Position,
                Values = o.Values
            }).ToList(),
            Status = product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Handle = product.Handle,
            PublishedAt = product.PublishedAt,
            SeoTitle = product.SeoTitle,
            SeoDescription = product.SeoDescription,
            MainImage = product.MainImage != null ? new ProductImageDto
            {
                Id = product.MainImage.Id,
                Url = product.MainImage.Url,
                AltText = product.MainImage.AltText,
                Position = product.MainImage.Position,
                Width = product.MainImage.Width,
                Height = product.MainImage.Height
            } : null,
            IsAvailable = product.IsAvailable,
            MinPrice = product.MinPrice,
            MaxPrice = product.MaxPrice,
            TotalInventory = product.TotalInventory
        };
    }
} 