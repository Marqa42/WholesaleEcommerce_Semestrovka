using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Application.Services;
using WholesaleEcommerce.Domain.Entities;

namespace WholesaleEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserSearchResponse), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<ActionResult<UserSearchResponse>> GetUsers([FromQuery] UserSearchRequest request)
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

            var user = new User { Id = userGuid, Role = currentUser.Role };
            var response = await _userService.SearchAsync(request, user);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to get users");
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting users");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting users",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get user by ID (Admin or own profile)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
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

            var user = new User { Id = userGuid, Role = currentUser.Role };
            var targetUser = await _userService.GetByIdAsync(id, user);
            
            if (targetUser == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "User not found",
                    Detail = "User not found in system",
                    Status = 404
                });
            }

            return Ok(targetUser);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to get user {UserId}", id);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user {UserId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting user",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Create new user (Admin only)
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user information</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User creation failed for email: {Email}", request.Email);
            return Conflict(new ProblemDetails
            {
                Title = "User creation failed",
                Detail = ex.Message,
                Status = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user for email: {Email}", request.Email);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while creating user",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Update user (Admin or own profile)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Updated user information</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
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

            var user = new User { Id = userGuid, Role = currentUser.Role };
            var updatedUser = await _userService.UpdateAsync(id, request, user);
            return Ok(updatedUser);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to update user {UserId}", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User update failed for user {UserId}", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Update failed",
                Detail = ex.Message,
                Status = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating user {UserId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while updating user",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult> DeleteUser(Guid id)
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

            var user = new User { Id = userGuid, Role = currentUser.Role };
            await _userService.DeleteAsync(id, user);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to delete user {UserId}", id);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "User deletion failed for user {UserId}", id);
            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = ex.Message,
                Status = 404
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting user {UserId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while deleting user",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get user count (Admin only)
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>User count</returns>
    [HttpGet("count")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<ActionResult<int>> GetUserCount([FromQuery] UserSearchRequest? request = null)
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

            var user = new User { Id = userGuid, Role = currentUser.Role };
            var count = await _userService.GetUserCountAsync(request, user);
            return Ok(count);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt to get user count");
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting user count");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting user count",
                Status = 500
            });
        }
    }
} 