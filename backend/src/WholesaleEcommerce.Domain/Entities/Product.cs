using System.ComponentModel.DataAnnotations;

namespace WholesaleEcommerce.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Vendor { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ProductType { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new();
    
    public List<ProductVariant> Variants { get; set; } = new();
    
    public List<ProductImage> Images { get; set; } = new();
    
    public List<ProductOption> Options { get; set; } = new();
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "draft"; // active, draft, archived
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [MaxLength(255)]
    public string Handle { get; set; } = string.Empty;
    
    public DateTime? PublishedAt { get; set; }
    
    [MaxLength(255)]
    public string? SeoTitle { get; set; }
    
    [MaxLength(500)]
    public string? SeoDescription { get; set; }

    // Computed properties
    public ProductImage? MainImage => Images.FirstOrDefault(img => img.Position == 1) ?? Images.FirstOrDefault();
    
    public bool IsAvailable => Variants.Any(variant => variant.IsAvailable);
    
    public decimal MinPrice => Variants.Any() ? Variants.Min(v => v.Price) : 0;
    
    public decimal MaxPrice => Variants.Any() ? Variants.Max(v => v.Price) : 0;
    
    public int TotalInventory => Variants.Sum(variant => variant.InventoryQuantity);

    // Methods
    public bool HasVariant(string sku) => Variants.Any(variant => variant.Sku == sku);
    
    public ProductVariant? GetVariant(string sku) => Variants.FirstOrDefault(variant => variant.Sku == sku);
    
    public ProductVariant? GetVariantById(Guid id) => Variants.FirstOrDefault(variant => variant.Id == id);
    
    public bool IsInStock(string sku)
    {
        var variant = GetVariant(sku);
        return variant?.InventoryQuantity > 0;
    }
    
    public void UpdateInventory(string sku, int quantity)
    {
        var variant = GetVariant(sku);
        if (variant != null)
        {
            variant.InventoryQuantity = Math.Max(0, variant.InventoryQuantity + quantity);
        }
    }
}

public class ProductVariant
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    public decimal? CompareAtPrice { get; set; }
    
    public int InventoryQuantity { get; set; }
    
    public decimal? Weight { get; set; }
    
    [MaxLength(10)]
    public string? WeightUnit { get; set; }
    
    [MaxLength(100)]
    public string? Option1 { get; set; }
    
    [MaxLength(100)]
    public string? Option2 { get; set; }
    
    [MaxLength(100)]
    public string? Option3 { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    public bool IsAvailable => InventoryQuantity > 0;
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

public class ProductImage
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Url { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? AltText { get; set; }
    
    public int Position { get; set; }
    
    public int Width { get; set; }
    
    public int Height { get; set; }
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}

public class ProductOption
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int Position { get; set; }
    
    public List<string> Values { get; set; } = new();
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
} 