using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;
using WholesaleEcommerce.Infrastructure.Data;

namespace WholesaleEcommerce.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.Orders)
            .ToListAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<(IEnumerable<User> Users, int TotalCount)> SearchAsync(
        UserSearchCriteria criteria,
        int page = 1,
        int pageSize = 20)
    {
        var query = _context.Users
            .Include(u => u.Orders)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(criteria.Search))
        {
            var searchTerm = criteria.Search.ToLower();
            query = query.Where(u => 
                u.Email.ToLower().Contains(searchTerm) ||
                u.FirstName.ToLower().Contains(searchTerm) ||
                u.LastName.ToLower().Contains(searchTerm) ||
                u.FullName.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrEmpty(criteria.Role))
        {
            query = query.Where(u => u.Role.ToLower() == criteria.Role.ToLower());
        }

        if (!string.IsNullOrEmpty(criteria.Status))
        {
            query = query.Where(u => u.Status.ToLower() == criteria.Status.ToLower());
        }

        if (criteria.CreatedFrom.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= criteria.CreatedFrom.Value);
        }

        if (criteria.CreatedTo.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= criteria.CreatedTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "email" => criteria.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" => criteria.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => criteria.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "role" => criteria.SortDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            "status" => criteria.SortDescending ? query.OrderByDescending(u => u.Status) : query.OrderBy(u => u.Status),
            "createdat" => criteria.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "updatedat" => criteria.SortDescending ? query.OrderByDescending(u => u.UpdatedAt) : query.OrderBy(u => u.UpdatedAt),
            "lastloginat" => criteria.SortDescending ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => criteria.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt)
        };

        // Apply pagination
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalCount);
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateRefreshTokenAsync(Guid userId, string? refreshToken, DateTime? refreshTokenExpiryTime)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role, int page = 1, int pageSize = 20)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.Role.ToLower() == role.ToLower())
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByStatusAsync(string status, int page = 1, int pageSize = 20)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.Status.ToLower() == status.ToLower())
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(UserSearchCriteria? criteria = null)
    {
        var query = _context.Users.AsQueryable();

        if (criteria != null)
        {
            // Apply the same filters as in SearchAsync
            if (!string.IsNullOrEmpty(criteria.Search))
            {
                var searchTerm = criteria.Search.ToLower();
                query = query.Where(u => 
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.FullName.ToLower().Contains(searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(criteria.Role))
            {
                query = query.Where(u => u.Role.ToLower() == criteria.Role.ToLower());
            }

            if (!string.IsNullOrEmpty(criteria.Status))
            {
                query = query.Where(u => u.Status.ToLower() == criteria.Status.ToLower());
            }

            if (criteria.CreatedFrom.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= criteria.CreatedFrom.Value);
            }

            if (criteria.CreatedTo.HasValue)
            {
                query = query.Where(u => u.CreatedAt <= criteria.CreatedTo.Value);
            }
        }

        return await query.CountAsync();
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(int page = 1, int pageSize = 20)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.Status == "active")
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetAdminsAsync()
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.Role == "admin")
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null)
    {
        var query = _context.Users.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return !await query.AnyAsync();
    }

    public async Task<IEnumerable<User>> GetUsersCreatedInDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .Where(u => u.CreatedAt >= fromDate && u.CreatedAt <= toDate)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUserCountByRoleAsync(string role)
    {
        return await _context.Users
            .Where(u => u.Role.ToLower() == role.ToLower())
            .CountAsync();
    }

    public async Task<int> GetUserCountByStatusAsync(string status)
    {
        return await _context.Users
            .Where(u => u.Status.ToLower() == status.ToLower())
            .CountAsync();
    }

    public async Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash)
    {
        return await _context.Users
            .Include(u => u.Orders)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.PasswordHash == passwordHash);
    }
} 