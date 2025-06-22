using System.ComponentModel.DataAnnotations;

namespace WholesaleEcommerce.Application.DTOs;

public class ProductSearchRequest
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public string? Vendor { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? Status { get; set; }
    public List<string>? Tags { get; set; }
    public string? SortBy { get; set; } = "createdat";
    public bool SortDescending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ProductSearchResponse
{
    public List<ProductDto> Products { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductOptionDto> Options { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Handle { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public bool Published { get; set; }
    // Computed properties
    public ProductImageDto? MainImage { get; set; }
    public bool IsAvailable { get; set; }
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public int TotalInventory { get; set; }
}

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int InventoryQuantity { get; set; }
    public string InventoryPolicy { get; set; } = "deny";
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public bool RequiresShipping { get; set; } = true;
    public bool Taxable { get; set; } = true;
    public string? Barcode { get; set; }
    public Dictionary<string, string> Options { get; set; } = new();
    // For compatibility:
    public string? Option1 { get; set; }
    public string? Option2 { get; set; }
    public string? Option3 { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
}

public class ProductImageDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int Position { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class ProductOptionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public List<string> Values { get; set; } = new();
}

public class CreateProductRequest
{
    [Required]
    [MinLength(3)]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Handle { get; set; } = string.Empty;
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string Vendor { get; set; } = string.Empty;
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public string ProductType { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public List<CreateProductVariantRequest> Variants { get; set; } = new();
    public List<CreateProductImageRequest> Images { get; set; } = new();
    public List<CreateProductOptionRequest> Options { get; set; } = new();
    public string Status { get; set; } = "active";
    public bool Published { get; set; } = true;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
}

public class UpdateProductRequest
{
    [MinLength(3)]
    [MaxLength(255)]
    public string? Title { get; set; }
    public string? Description { get; set; }
    [MinLength(3)]
    [MaxLength(100)]
    public string? Handle { get; set; }
    [MinLength(2)]
    [MaxLength(100)]
    public string? Vendor { get; set; }
    [MinLength(2)]
    [MaxLength(100)]
    public string? ProductType { get; set; }
    public List<string>? Tags { get; set; }
    public List<ProductVariantDto>? Variants { get; set; }
    public List<ProductImageDto>? Images { get; set; }
    public List<ProductOptionDto>? Options { get; set; }
    public string? Status { get; set; }
    public bool? Published { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
}

public class CreateProductVariantRequest
{
    public string Title { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public int InventoryQuantity { get; set; }
    public string InventoryPolicy { get; set; } = "deny";
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }
    public bool RequiresShipping { get; set; } = true;
    public bool Taxable { get; set; } = true;
    public string? Barcode { get; set; }
    public string? Option1 { get; set; }
    public string? Option2 { get; set; }
    public string? Option3 { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateProductImageRequest
{
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int Position { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class CreateProductOptionRequest
{
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public List<string> Values { get; set; } = new();
}

public class InventoryUpdateRequest
{
    [Required]
    public string Sku { get; set; } = string.Empty;

    [Required]
    public int Quantity { get; set; }
}

public class ProductCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProductVendorDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
} 