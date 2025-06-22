using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;
using WholesaleEcommerce.Infrastructure.Data;

namespace WholesaleEcommerce.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Variants)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Variants)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Images)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .ToListAsync();
    }

    public async Task<Order> AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<(IEnumerable<Order> Orders, int TotalCount)> SearchAsync(
        OrderSearchCriteria criteria,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(criteria.Search))
        {
            var searchTerm = criteria.Search.ToLower();
            query = query.Where(o => 
                o.OrderNumber.ToLower().Contains(searchTerm) ||
                o.User!.Email.ToLower().Contains(searchTerm) ||
                o.User!.FullName.ToLower().Contains(searchTerm) ||
                o.ShippingFirstName.ToLower().Contains(searchTerm) ||
                o.ShippingLastName.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrEmpty(criteria.Status))
        {
            query = query.Where(o => o.Status.ToLower() == criteria.Status.ToLower());
        }

        if (!string.IsNullOrEmpty(criteria.PaymentStatus))
        {
            query = query.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == criteria.PaymentStatus.ToLower());
        }

        if (criteria.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == criteria.UserId.Value);
        }

        if (criteria.MinTotal.HasValue)
        {
            query = query.Where(o => o.TotalAmount >= criteria.MinTotal.Value);
        }

        if (criteria.MaxTotal.HasValue)
        {
            query = query.Where(o => o.TotalAmount <= criteria.MaxTotal.Value);
        }

        if (criteria.CreatedFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= criteria.CreatedFrom.Value);
        }

        if (criteria.CreatedTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= criteria.CreatedTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "ordernumber" => criteria.SortDescending ? query.OrderByDescending(o => o.OrderNumber) : query.OrderBy(o => o.OrderNumber),
            "total" => criteria.SortDescending ? query.OrderByDescending(o => o.TotalAmount) : query.OrderBy(o => o.TotalAmount),
            "status" => criteria.SortDescending ? query.OrderByDescending(o => o.Status) : query.OrderBy(o => o.Status),
            "paymentstatus" => criteria.SortDescending ? query.OrderByDescending(o => o.PaymentStatus) : query.OrderBy(o => o.PaymentStatus),
            "createdat" => criteria.SortDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt),
            "updatedat" => criteria.SortDescending ? query.OrderByDescending(o => o.UpdatedAt) : query.OrderBy(o => o.UpdatedAt),
            _ => criteria.SortDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt)
        };

        // Apply pagination
        var orders = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (orders, totalCount);
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Variants)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p!.Images)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(string status, int page = 1, int pageSize = 20)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status.ToLower() == status.ToLower())
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByPaymentStatusAsync(string paymentStatus, int page = 1, int pageSize = 20)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == paymentStatus.ToLower())
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(Guid orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePaymentStatusAsync(Guid orderId, string paymentStatus)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.PaymentStatus = paymentStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var prefix = DateTime.UtcNow.ToString("yyyyMMdd");
        var lastOrder = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSequence = lastOrder.OrderNumber.Substring(prefix.Length);
            if (int.TryParse(lastSequence, out int lastNumber))
            {
                sequence = lastNumber + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }

    public async Task<int> GetCountAsync(OrderSearchCriteria? criteria = null)
    {
        var query = _context.Orders.AsQueryable();

        if (criteria != null)
        {
            // Apply the same filters as in SearchAsync
            if (!string.IsNullOrEmpty(criteria.Search))
            {
                var searchTerm = criteria.Search.ToLower();
                query = query.Where(o => 
                    o.OrderNumber.ToLower().Contains(searchTerm) ||
                    o.User!.Email.ToLower().Contains(searchTerm) ||
                    o.User!.FullName.ToLower().Contains(searchTerm) ||
                    o.ShippingFirstName.ToLower().Contains(searchTerm) ||
                    o.ShippingLastName.ToLower().Contains(searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(criteria.Status))
            {
                query = query.Where(o => o.Status.ToLower() == criteria.Status.ToLower());
            }

            if (!string.IsNullOrEmpty(criteria.PaymentStatus))
            {
                query = query.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == criteria.PaymentStatus.ToLower());
            }

            if (criteria.UserId.HasValue)
            {
                query = query.Where(o => o.UserId == criteria.UserId.Value);
            }

            if (criteria.MinTotal.HasValue)
            {
                query = query.Where(o => o.TotalAmount >= criteria.MinTotal.Value);
            }

            if (criteria.MaxTotal.HasValue)
            {
                query = query.Where(o => o.TotalAmount <= criteria.MaxTotal.Value);
            }

            if (criteria.CreatedFrom.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= criteria.CreatedFrom.Value);
            }

            if (criteria.CreatedTo.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= criteria.CreatedTo.Value);
            }
        }

        return await query.CountAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Orders.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        return await query.SumAsync(o => o.TotalAmount);
    }

    public async Task<int> GetOrderCountAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Orders.AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersInDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetOrderCountByStatusAsync(string status)
    {
        return await _context.Orders.CountAsync(o => o.Status.ToLower() == status.ToLower());
    }

    public async Task<int> GetOrderCountByPaymentStatusAsync(string paymentStatus)
    {
        return await _context.Orders.CountAsync(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == paymentStatus.ToLower());
    }

    public async Task<int> GetCountByUserIdAsync(Guid userId)
    {
        return await _context.Orders.CountAsync(o => o.UserId == userId);
    }

    public async Task UpdateShippingInfoAsync(Guid orderId, string trackingNumber, string? trackingUrl)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.TrackingNumber = trackingNumber;
            order.TrackingUrl = trackingUrl;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePaymentInfoAsync(Guid orderId, string paymentStatus, string? transactionId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.PaymentStatus = paymentStatus;
            order.TransactionId = transactionId;
            await _context.SaveChangesAsync();
        }
    }
} 