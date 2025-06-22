using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> AddAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByOrderNumberAsync(string orderNumber);
    
    // User orders
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<int> GetCountByUserIdAsync(Guid userId);
    
    // Search and filtering
    Task<(IEnumerable<Order> Orders, int TotalCount)> SearchAsync(
        OrderSearchCriteria criteria,
        int page = 1,
        int pageSize = 20);
    
    // Status management
    Task UpdateStatusAsync(Guid orderId, string status);
    Task UpdateShippingInfoAsync(Guid orderId, string trackingNumber, string? trackingUrl);
    Task UpdatePaymentInfoAsync(Guid orderId, string paymentStatus, string? transactionId);
    
    // Counts
    Task<int> GetCountAsync(OrderSearchCriteria? criteria = null);
    Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
} 