using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Interface;

namespace Server.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductInterface _productService;
        public ProductController(IProductInterface productService)
        {
            _productService = productService;
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(new { message = "Products retrieved successfully", data = products });
            }
            catch (Exception)
            {
                Console.WriteLine(new{error = "An unexpected error occurred."});
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [Authorize(Policy = "OwnerPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(new { message = "Product retrieved successfully", data = product });
            }
            catch (Exception e)
            {
                Console.WriteLine(new{error = e.Message});
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpPost]
        [RequestSizeLimit(10_000_000)]
        [RequestFormLimits(MultipartBodyLengthLimit = 10_000_000)]
        public async Task<IActionResult> CreateProduct(
    [FromForm] ProductDTOs.CreateProductDTOs dto
)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _productService.CreateProduct(dto);
                return Ok(new { message = "Product added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception e)
            {
                Console.WriteLine(new{error = e.Message});
                return StatusCode(500, new { error = e.Message });
            }
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id,[FromForm] ProductDTOs.UpdateProductDTOs _updateProductDTOs)
        {
            try
            {
                await _productService.UpdateProductById(_updateProductDTOs, id);
                return Ok(new {message = "Product update successfully"});
            }
            catch (Exception e)
            {
                Console.WriteLine(new{error = e.Message});
                return NotFound(new { error = e.Message });
            }
        }
        
        [Authorize(Policy = "OwnerOnly")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest(new { error = "Provide name to search" });
            }

            var products = await _productService.SearchProduct(productName);

            return Ok(new
            {
                message = products.Any()
                    ? "Products retrieved successfully"
                    : "No products found",
                data = products
            });
        }
        
        [Authorize(Policy = "OwnerOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> ArchiveProduct(Guid id)
        {
            try
            {
                await _productService.ArchiveProductById(id);
                return Ok(new { message = "Product archived successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(new{error = e.Message});
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpDelete("archive/{id}/restore")]
        public async Task<IActionResult> RestoreProduct(Guid id)
        {
            try
            {
                await _productService.RestoreArchiveProduct(id);
                return Ok(new { message = "Product restored successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(new{error = e.Message});
                return NotFound(new {error = e.Message });
            }
        }
        
        [Authorize(Policy = "OwnerOnly")]
        [HttpGet("archive")]
        public async Task<IActionResult> GetAllArchivesProduct()
        {
            var archives = await _productService.GetAllArchivesProduct();
            return Ok(new 
                { message = archives.Any() 
                    ? "Archive retrieved successfully" 
                    : "Archives not found",
                    data = archives
                });
        }

        [Authorize(Policy = "OwnerOnly")]
        [HttpDelete("archive/{id}")]
        public async Task<IActionResult> GetArchiveProductById(Guid id)
        {
            try
            {
                await _productService.DeleteArchiveProduct(id);
                return Ok(new { message = "Archive deleted successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return NotFound(new { error = e.Message });
            }
        }
        
    }
}
