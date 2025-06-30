using LayeredApp.Application.DTOs;
using LayeredApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LayeredApp.Presentation.Controllers
{
    // ===========================================
    // PRESENTATION LAYER - API CONTROLLER
    // ===========================================
    // This controller handles HTTP requests and responses
    // It's responsible for:
    // - Receiving HTTP requests
    // - Validating input
    // - Calling application services
    // - Returning HTTP responses
    // - Error handling
    //
    // The Presentation layer contains:
    // - Controllers (Web API, MVC, etc.)
    // - View models
    // - HTTP-specific logic
    // - Input validation

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving products", details = ex.Message });
            }
        }

        // GET: api/products/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetActive()
        {
            try
            {
                var products = await _productService.GetActiveProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving active products", details = ex.Message });
            }
        }

        // GET: api/products/low-stock
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock([FromQuery] int threshold = 10)
        {
            try
            {
                var products = await _productService.GetLowStockProductsAsync(threshold);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving low stock products", details = ex.Message });
            }
        }

        // GET: api/products/search?name=laptop
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Search([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { error = "Search term cannot be empty" });

            try
            {
                var products = await _productService.SearchByNameAsync(name);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while searching products", details = ex.Message });
            }
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound(new { error = $"Product with ID {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the product", details = ex.Message });
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the product", details = ex.Message });
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateAsync(id, updateDto);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the product", details = ex.Message });
            }
        }

        // PATCH: api/products/5/stock
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateStockAsync(id, stockDto);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating product stock", details = ex.Message });
            }
        }

        // POST: api/products/5/activate
        [HttpPost("{id}/activate")]
        public async Task<ActionResult<ProductDto>> Activate(int id)
        {
            try
            {
                var product = await _productService.ActivateAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while activating the product", details = ex.Message });
            }
        }

        // POST: api/products/5/deactivate
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult<ProductDto>> Deactivate(int id)
        {
            try
            {
                var product = await _productService.DeactivateAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deactivating the product", details = ex.Message });
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the product", details = ex.Message });
            }
        }
    }
}