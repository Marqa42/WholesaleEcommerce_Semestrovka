using Microsoft.Extensions.Logging;
using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Application.Services;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;

namespace WholesaleEcommerce.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IJwtService jwtService, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, User? currentUser = null)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return null;

        // Users can only see their own profile unless they are admin
        if (currentUser == null || (!currentUser.IsAdmin && currentUser.Id != id))
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, User? currentUser = null)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null)
            return null;

        // Users can only see their own profile unless they are admin
        if (currentUser == null || (!currentUser.IsAdmin && currentUser.Email != email))
            return null;

        return MapToDto(user);
    }

    public async Task<UserSearchResponse> SearchAsync(UserSearchRequest request, User? currentUser = null)
    {
        // Only admins can search users
        if (currentUser == null || !currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can search users");
        }

        var criteria = new UserSearchCriteria
        {
            Search = request.Search,
            Role = request.Role,
            Status = request.Status,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        var (users, totalCount) = await _userRepository.SearchAsync(criteria, request.Page, request.PageSize);
        
        var userDtos = users.Select(MapToDto).ToList();
        
        return new UserSearchResponse
        {
            Users = userDtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        // Check if email already exists
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException($"User with email '{request.Email}' already exists");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, User currentUser)
    {
        // Users can only update their own profile unless they are admin
        if (!currentUser.IsAdmin && currentUser.Id != id)
        {
            throw new UnauthorizedAccessException("You can only update your own profile");
        }

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Update properties if provided
        if (!string.IsNullOrEmpty(request.FirstName))
            existingUser.FirstName = request.FirstName;
        
        if (!string.IsNullOrEmpty(request.LastName))
            existingUser.LastName = request.LastName;
        
        if (!string.IsNullOrEmpty(request.Role))
        {
            // Only admins can change roles
            if (!currentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can change user roles");
            }
            existingUser.Role = request.Role;
        }
        
        if (!string.IsNullOrEmpty(request.Status))
        {
            // Only admins can change status
            if (!currentUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only admins can change user status");
            }
            existingUser.Status = request.Status;
        }

        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return MapToDto(updatedUser);
    }

    public async Task DeleteAsync(Guid id, User currentUser)
    {
        // Only admins can delete users
        if (!currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can delete users");
        }

        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found");
        }

        await _userRepository.DeleteAsync(id);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is not active");
        }

        // Update last login
        await _userRepository.UpdateLastLoginAsync(user.Id);

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 days
        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = MapToDto(user)
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Find user by refresh token
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is not active");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save new refresh token
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            User = MapToDto(user)
        };
    }

    public async Task LogoutAsync(User currentUser)
    {
        // Clear refresh token
        await _userRepository.UpdateRefreshTokenAsync(currentUser.Id, null, null);
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<int> GetUserCountAsync(UserSearchRequest? request = null, User? currentUser = null)
    {
        // Only admins can get user count
        if (currentUser == null || !currentUser.IsAdmin)
        {
            throw new UnauthorizedAccessException("Only admins can get user count");
        }

        UserSearchCriteria? criteria = null;
        
        if (request != null)
        {
            criteria = new UserSearchCriteria
            {
                Search = request.Search,
                Role = request.Role,
                Status = request.Status,
                CreatedFrom = request.CreatedFrom,
                CreatedTo = request.CreatedTo
            };
        }

        return await _userRepository.GetCountAsync(criteria);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            FullName = user.FullName,
            IsAdmin = user.IsAdmin,
            IsActive = user.IsActive
        };
    }
} 