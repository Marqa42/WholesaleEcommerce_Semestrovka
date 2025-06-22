using System.ComponentModel.DataAnnotations;

namespace WholesaleEcommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "pending"; // pending, confirmed, processing, shipped, delivered, cancelled
    
    [Required]
    public decimal Subtotal { get; set; }
    
    [Required]
    public decimal TaxAmount { get; set; }
    
    [Required]
    public decimal ShippingAmount { get; set; }
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; } = "USD";
    
    // Shipping information
    [Required]
    [MaxLength(100)]
    public string ShippingFirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ShippingLastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string ShippingAddress1 { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? ShippingAddress2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string ShippingCity { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ShippingState { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string ShippingZipCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string ShippingCountry { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [MaxLength(20)]
    public string ShippingPhone { get; set; } = string.Empty;
    
    // Billing information
    [Required]
    [MaxLength(100)]
    public string BillingFirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string BillingLastName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string BillingAddress1 { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? BillingAddress2 { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string BillingCity { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string BillingState { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string BillingZipCode { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string BillingCountry { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    [MaxLength(20)]
    public string BillingPhone { get; set; } = string.Empty;
    
    // Payment information
    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
    
    [MaxLength(100)]
    public string? PaymentStatus { get; set; }
    
    [MaxLength(255)]
    public string? TransactionId { get; set; }
    
    // Shipping information
    [MaxLength(100)]
    public string? ShippingMethod { get; set; }
    
    [MaxLength(255)]
    public string? TrackingNumber { get; set; }
    
    [MaxLength(255)]
    public string? TrackingUrl { get; set; }
    
    public DateTime? ShippedAt { get; set; }
    
    public DateTime? DeliveredAt { get; set; }
    
    // Notes
    [MaxLength(1000)]
    public string? CustomerNotes { get; set; }
    
    [MaxLength(1000)]
    public string? InternalNotes { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public List<OrderItem> OrderItems { get; set; } = new();

    // Computed properties
    public int TotalItems => OrderItems.Sum(item => item.Quantity);
    
    public bool IsShipped => !string.IsNullOrEmpty(TrackingNumber);
    
    public bool IsDelivered => DeliveredAt.HasValue;
    
    public bool IsCancelled => Status == "cancelled";
    
    public bool CanBeCancelled => Status == "pending" || Status == "confirmed";
    
    public bool CanBeShipped => Status == "confirmed" || Status == "processing";
}

public class OrderItem
{
    public Guid Id { get; set; }
    
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string ProductTitle { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Sku { get; set; } = string.Empty;
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    [MaxLength(100)]
    public string? Option1 { get; set; }
    
    [MaxLength(100)]
    public string? Option2 { get; set; }
    
    [MaxLength(100)]
    public string? Option3 { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 