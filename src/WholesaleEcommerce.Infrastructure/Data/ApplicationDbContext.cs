using Microsoft.EntityFrameworkCore;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductOption> ProductOptions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Vendor).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Handle).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SeoTitle).HasMaxLength(255);
            entity.Property(e => e.SeoDescription).HasMaxLength(500);
            
            entity.HasIndex(e => e.Handle).IsUnique();
            entity.HasIndex(e => e.Vendor);
            entity.HasIndex(e => e.ProductType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // ProductVariant configuration
        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CompareAtPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(18,2)");
            entity.Property(e => e.WeightUnit).HasMaxLength(10);
            entity.Property(e => e.Option1).HasMaxLength(100);
            entity.Property(e => e.Option2).HasMaxLength(100);
            entity.Property(e => e.Option3).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Variants)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.Sku);
            entity.HasIndex(e => e.ProductId);
        });

        // ProductImage configuration
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AltText).HasMaxLength(255);
            
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Images)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.Position);
        });

        // ProductOption configuration
        modelBuilder.Entity<ProductOption>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.Product)
                .WithMany(e => e.Options)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.Position);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ShippingAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            
            // Shipping information
            entity.Property(e => e.ShippingFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingLastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingAddress1).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ShippingAddress2).HasMaxLength(255);
            entity.Property(e => e.ShippingCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingState).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingZipCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ShippingCountry).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShippingPhone).IsRequired().HasMaxLength(20);
            
            // Billing information
            entity.Property(e => e.BillingFirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BillingLastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BillingAddress1).IsRequired().HasMaxLength(255);
            entity.Property(e => e.BillingAddress2).HasMaxLength(255);
            entity.Property(e => e.BillingCity).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BillingState).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BillingZipCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BillingCountry).IsRequired().HasMaxLength(100);
            entity.Property(e => e.BillingPhone).IsRequired().HasMaxLength(20);
            
            // Payment and shipping
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(100);
            entity.Property(e => e.TransactionId).HasMaxLength(255);
            entity.Property(e => e.ShippingMethod).HasMaxLength(100);
            entity.Property(e => e.TrackingNumber).HasMaxLength(255);
            entity.Property(e => e.TrackingUrl).HasMaxLength(255);
            
            // Notes
            entity.Property(e => e.CustomerNotes).HasMaxLength(1000);
            entity.Property(e => e.InternalNotes).HasMaxLength(1000);
            
            entity.HasOne(e => e.User)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductTitle).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Option1).HasMaxLength(100);
            entity.Property(e => e.Option2).HasMaxLength(100);
            entity.Property(e => e.Option3).HasMaxLength(100);
            
            entity.HasOne(e => e.Order)
                .WithMany(e => e.OrderItems)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ProductVariant)
                .WithMany()
                .HasForeignKey(e => e.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.ProductVariantId);
        });

        // Configure JSON columns for Tags and Values
        modelBuilder.Entity<Product>()
            .Property(e => e.Tags)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<ProductOption>()
            .Property(e => e.Values)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
} 