using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WholesaleEcommerce.Application.DTOs;
using WholesaleEcommerce.Application.Services;
using WholesaleEcommerce.Domain.Entities;
using WholesaleEcommerce.Domain.Repositories;

namespace WholesaleEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, IUserRepository userRepository, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get products with search and filtering
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>List of products</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ProductSearchResponse), 200)]
    public async Task<ActionResult<ProductSearchResponse>> GetProducts([FromQuery] ProductSearchRequest request)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var response = await _productService.SearchAsync(request, currentUser);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting products");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting products",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var product = await _productService.GetByIdAsync(id, currentUser);
            if (product == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Product not found",
                    Detail = "Product not found in system",
                    Status = 404
                });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting product {ProductId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting product",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get product by handle
    /// </summary>
    /// <param name="handle">Product handle</param>
    /// <returns>Product information</returns>
    [HttpGet("handle/{handle}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDto>> GetProductByHandle(string handle)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var product = await _productService.GetByHandleAsync(handle, currentUser);
            if (product == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Product not found",
                    Detail = "Product not found in system",
                    Status = 404
                });
            }

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting product by handle {Handle}", handle);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting product",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Create new product (Authenticated users)
    /// </summary>
    /// <param name="request">Product creation data</param>
    /// <returns>Created product information</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProductDto), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "User not authenticated",
                    Status = 401
                });
            }

            var product = await _productService.CreateAsync(request, currentUser);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Product creation failed for handle: {Handle}", request.Handle);
            return Conflict(new ProblemDetails
            {
                Title = "Product creation failed",
                Detail = ex.Message,
                Status = 409
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating product for handle: {Handle}", request.Handle);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while creating product",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Update product (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Updated product data</param>
    /// <returns>Updated product information</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "User not authenticated",
                    Status = 401
                });
            }

            var product = await _productService.UpdateAsync(id, request, currentUser);
            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Product update failed for product {ProductId}", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Update failed",
                Detail = ex.Message,
                Status = 400
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating product {ProductId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while updating product",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Delete product (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "User not authenticated",
                    Status = 401
                });
            }

            await _productService.DeleteAsync(id, currentUser);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Product deletion failed for product {ProductId}", id);
            return NotFound(new ProblemDetails
            {
                Title = "Product not found",
                Detail = ex.Message,
                Status = 404
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting product {ProductId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while deleting product",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    /// <param name="category">Category name</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of products in category</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(List<ProductDto>), 200)]
    public async Task<ActionResult<List<ProductDto>>> GetProductsByCategory(string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var products = await _productService.GetByCategoryAsync(category, page, pageSize, currentUser);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting products by category {Category}", category);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting products by category",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get products by vendor
    /// </summary>
    /// <param name="vendor">Vendor name</param>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>List of products by vendor</returns>
    [HttpGet("vendor/{vendor}")]
    [ProducesResponseType(typeof(List<ProductDto>), 200)]
    public async Task<ActionResult<List<ProductDto>>> GetProductsByVendor(string vendor, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var products = await _productService.GetByVendorAsync(vendor, page, pageSize, currentUser);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting products by vendor {Vendor}", vendor);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting products by vendor",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get featured products
    /// </summary>
    /// <param name="limit">Number of products to return</param>
    /// <returns>List of featured products</returns>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(List<ProductDto>), 200)]
    public async Task<ActionResult<List<ProductDto>>> GetFeaturedProducts([FromQuery] int limit = 10)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var products = await _productService.GetFeaturedAsync(limit, currentUser);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting featured products");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting featured products",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get related products
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="limit">Number of products to return</param>
    /// <returns>List of related products</returns>
    [HttpGet("{id}/related")]
    [ProducesResponseType(typeof(List<ProductDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<List<ProductDto>>> GetRelatedProducts(Guid id, [FromQuery] int limit = 5)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var products = await _productService.GetRelatedAsync(id, limit, currentUser);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting related products for product {ProductId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting related products",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Update product inventory (Admin only)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="request">Inventory update data</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/inventory")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    public async Task<ActionResult> UpdateInventory(Guid id, [FromBody] InventoryUpdateRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Detail = "User not authenticated",
                    Status = 401
                });
            }

            await _productService.UpdateInventoryAsync(id, request.Sku, request.Quantity, currentUser);
            return Ok(new { message = "Inventory updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating inventory for product {ProductId}", id);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while updating inventory",
                Status = 500
            });
        }
    }

    /// <summary>
    /// Get product count
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <returns>Product count</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<ActionResult<int>> GetProductCount([FromQuery] ProductSearchRequest? request = null)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var count = await _productService.GetCountAsync(request, currentUser);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting product count");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Internal server error",
                Detail = "An unexpected error occurred while getting product count",
                Status = 500
            });
        }
    }

    private async Task<User?> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        return await _userRepository.GetByIdAsync(userId);
    }
} 