using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id, User? currentUser = null);
    Task<UserDto?> GetByEmailAsync(string email, User? currentUser = null);
    Task<UserSearchResponse> SearchAsync(UserSearchRequest request, User? currentUser = null);
    
    Task<UserDto> CreateAsync(CreateUserRequest request);
    Task<UserDto> UpdateAsync(Guid id, UpdateUserRequest request, User currentUser);
    Task DeleteAsync(Guid id, User currentUser);
    
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(User currentUser);
    
    Task<bool> ValidatePasswordAsync(string email, string password);
    Task<int> GetUserCountAsync(UserSearchRequest? request = null, User? currentUser = null);
} 