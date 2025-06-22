using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    
    // Authentication
    Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash);
    Task UpdateLastLoginAsync(Guid userId);
    Task UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? expiryTime);
    
    // Search and filtering
    Task<(IEnumerable<User> Users, int TotalCount)> SearchAsync(
        UserSearchCriteria criteria,
        int page = 1,
        int pageSize = 20);
    
    // Counts
    Task<int> GetCountAsync(UserSearchCriteria? criteria = null);
} 