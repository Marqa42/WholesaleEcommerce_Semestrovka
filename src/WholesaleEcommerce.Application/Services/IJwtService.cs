using System.Security.Claims;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.Application.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    string GetUserIdFromToken(string token);
} 