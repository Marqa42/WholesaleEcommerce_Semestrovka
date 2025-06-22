using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Application.Services;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.LoginAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication failed",
                Detail = ex.Message,
                Status = 401
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred during login",
                Status = 500
            });
        }
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">User registration data</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
            return Conflict(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = ex.Message,
                Status = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for email: {Email}", request.Email);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred during registration",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _userService.RefreshTokenAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(new ProblemDetails
            {
                Title = "Token refresh failed",
                Detail = ex.Message,
                Status = 401
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token refresh");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred during token refresh",
                Status = 500
            });
        }
    }

    /// <summary>
    /// User logout
    /// </summary>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "User ID not found in token",
                    Status = 401
                });
            }

            // Get current user from service
            var currentUser = await _userService.GetByIdAsync(userGuid);
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = "User not found in system",
                    Status = 401
                });
            }

            // Create a User entity for the service
            var user = new User { Id = userGuid };
            await _userService.LogoutAsync(user);

            return Ok(new { message = "Successfully logged out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred during logout",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "User ID not found in token",
                    Status = 401
                });
            }

            var user = await _userService.GetByIdAsync(userGuid);
            if (user == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = "User not found in system",
                    Status = 404
                });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user profile");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting user profile",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Updated user data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "User ID not found in token",
                    Status = 401
                });
            }

            // Get current user from service
            var currentUser = await _userService.GetByIdAsync(userGuid);
            if (currentUser == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = "User not found in system",
                    Status = 404
                });
            }

            // Create a User entity for the service
            var user = new User 
            { 
                Id = userGuid,
                Role = currentUser.Role
            };

            var updatedUser = await _userService.UpdateAsync(userGuid, request, user);
            return Ok(updatedUser);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized profile update attempt");
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = 401
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Profile update failed");
            return BadRequest(new ProblemDetails
            {
                Title = "Update failed",
                Detail = ex.Message,
                Status = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user profile");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while updating user profile",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Validate password for current user
    /// </summary>
    /// <param name="request">Password validation request</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate-password")]
    [Authorize]
    [ProducesResponseType(typeof(PasswordValidationResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult<PasswordValidationResponse>> ValidatePassword([FromBody] PasswordValidationRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid token",
                    Detail = "User ID not found in token",
                    Status = 401
                });
            }

            // Get current user email
            var currentUser = await _userService.GetByIdAsync(userGuid);
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = "User not found in system",
                    Status = 401
                });
            }

            var isValid = await _userService.ValidatePasswordAsync(currentUser.Email, request.Password);
            
            return Ok(new PasswordValidationResponse
            {
                IsValid = isValid
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating password");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while validating password",
                Status = 500
            });
        }
    }
}

// Additional DTOs for authentication
public class PasswordValidationRequest
{
    public string Password { get; set; } = string.Empty;
}

public class PasswordValidationResponse
{
    public bool IsValid { get; set; }
} 