using Microsoft.EntityFrameworkCore;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Infrastructure.Data;

namespace WholesaleEcommerce.API;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Проверяем, есть ли уже данные
        if (await context.Users.AnyAsync())
        {
            return; // Данные уже есть
        }

        // Создаем тестовых пользователей
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@wholesale.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Admin",
                LastName = "User",
                Role = "admin",
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsOnline = false,
                LastSeenAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "wholesale@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("wholesale123"),
                FirstName = "Wholesale",
                LastName = "Customer",
                Role = "wholesale",
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsOnline = false,
                LastSeenAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "retail@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("retail123"),
                FirstName = "Retail",
                LastName = "Customer",
                Role = "retail",
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsOnline = false,
                LastSeenAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);

        // Создаем тестовые продукты
        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Title = "Premium LED Headlights",
                Description = "High-quality LED headlights for automotive use. Bright, energy-efficient, and long-lasting.",
                Vendor = "AutoTech Pro",
                ProductType = "automotive",
                Tags = new List<string> { "led", "headlights", "automotive", "premium" },
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Handle = "premium-led-headlights",
                PublishedAt = DateTime.UtcNow,
                SeoTitle = "Premium LED Headlights - AutoTech Pro",
                SeoDescription = "High-quality LED headlights for automotive use"
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Title = "Professional Drill Set",
                Description = "Complete professional drill set with multiple attachments and carrying case.",
                Vendor = "ToolMaster",
                ProductType = "tools",
                Tags = new List<string> { "drill", "tools", "professional", "set" },
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Handle = "professional-drill-set",
                PublishedAt = DateTime.UtcNow,
                SeoTitle = "Professional Drill Set - ToolMaster",
                SeoDescription = "Complete professional drill set with multiple attachments"
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Title = "Smart Home Security Camera",
                Description = "Wireless security camera with night vision and motion detection.",
                Vendor = "SecureTech",
                ProductType = "electronics",
                Tags = new List<string> { "security", "camera", "smart", "wireless" },
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Handle = "smart-home-security-camera",
                PublishedAt = DateTime.UtcNow,
                SeoTitle = "Smart Home Security Camera - SecureTech",
                SeoDescription = "Wireless security camera with night vision and motion detection"
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Title = "Industrial LED Work Light",
                Description = "Bright industrial work light with adjustable stand and long battery life.",
                Vendor = "LightPro",
                ProductType = "lighting",
                Tags = new List<string> { "led", "work-light", "industrial", "adjustable" },
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Handle = "industrial-led-work-light",
                PublishedAt = DateTime.UtcNow,
                SeoTitle = "Industrial LED Work Light - LightPro",
                SeoDescription = "Bright industrial work light with adjustable stand"
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Title = "Premium Motorcycle Helmet",
                Description = "High-quality motorcycle helmet with advanced safety features and comfortable fit.",
                Vendor = "RideSafe",
                ProductType = "automotive",
                Tags = new List<string> { "helmet", "motorcycle", "safety", "premium" },
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Handle = "premium-motorcycle-helmet",
                PublishedAt = DateTime.UtcNow,
                SeoTitle = "Premium Motorcycle Helmet - RideSafe",
                SeoDescription = "High-quality motorcycle helmet with advanced safety features"
            }
        };

        await context.Products.AddRangeAsync(products);

        // Создаем тестовые заказы
        var orders = new List<Order>
        {
            new Order
            {
                Id = Guid.NewGuid(),
                UserId = users[1].Id, // wholesale customer
                OrderNumber = "ORD-2024-001",
                Status = "pending",
                PaymentStatus = "pending",
                PaymentMethod = "credit_card",
                Subtotal = 299.99m,
                TaxAmount = 29.99m,
                TotalAmount = 329.98m,
                Currency = "USD",
                ShippingMethod = "standard",
                ShippingAmount = 15.00m,
                ShippingFirstName = "Wholesale",
                ShippingLastName = "Customer",
                ShippingAddress1 = "123 Business St",
                ShippingCity = "Business City",
                ShippingState = "BC",
                ShippingZipCode = "12345",
                ShippingCountry = "USA",
                ShippingPhone = "+1-555-0123",
                BillingFirstName = "Wholesale",
                BillingLastName = "Customer",
                BillingAddress1 = "123 Business St",
                BillingCity = "Business City",
                BillingState = "BC",
                BillingZipCode = "12345",
                BillingCountry = "USA",
                BillingPhone = "+1-555-0123",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Order
            {
                Id = Guid.NewGuid(),
                UserId = users[2].Id, // retail customer
                OrderNumber = "ORD-2024-002",
                Status = "completed",
                PaymentStatus = "paid",
                PaymentMethod = "paypal",
                Subtotal = 149.99m,
                TaxAmount = 15.00m,
                TotalAmount = 164.99m,
                Currency = "USD",
                ShippingMethod = "express",
                ShippingAmount = 25.00m,
                ShippingFirstName = "Retail",
                ShippingLastName = "Customer",
                ShippingAddress1 = "456 Consumer Ave",
                ShippingCity = "Consumer Town",
                ShippingState = "CT",
                ShippingZipCode = "67890",
                ShippingCountry = "USA",
                ShippingPhone = "+1-555-0456",
                BillingFirstName = "Retail",
                BillingLastName = "Customer",
                BillingAddress1 = "456 Consumer Ave",
                BillingCity = "Consumer Town",
                BillingState = "CT",
                BillingZipCode = "67890",
                BillingCountry = "USA",
                BillingPhone = "+1-555-0456",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                ShippedAt = DateTime.UtcNow.AddDays(-2),
                DeliveredAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        await context.Orders.AddRangeAsync(orders);

        await context.SaveChangesAsync();
    }
} 