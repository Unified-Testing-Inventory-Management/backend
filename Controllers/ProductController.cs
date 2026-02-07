using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;

namespace Server.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            return Ok(new { message = "Products retrieved successfully", data = products });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(new { message = "Product retrieved successfully", data = product });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message });
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
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
            catch (ProductExceptions.InvalidProductPriceException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (ProductExceptions.InvalidStockQuantityException e)
            {
                return BadRequest(new { error = e.Message });
            }
            catch (ProductExceptions.ProductAlreadyExists e)
            {
                Console.WriteLine(new { error = e.Message });
                return Conflict(new { error = e.Message });
            }

        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductDTOs.UpdateProductDTOs _updateProductDTOs)
        {
            try
            {
                await _productService.UpdateProductById(_updateProductDTOs, id);
                return Ok(new { message = "Product update successfully" });
            }
            catch (ProductExceptions.InvalidProductPriceException e)
            {
                Console.WriteLine(new { error = e.Message });
                return NotFound(new { error = e.Message });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message });
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
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

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> ArchiveProduct(Guid id)
        {
            try
            {
                await _productService.ArchiveProductById(id);
                return Ok(new { message = "Product archived successfully" });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message });
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("archive/search")]
        public async Task<IActionResult> SearchArchiveProduct([FromQuery] string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest(new { error = "Provide name to search" });
            }

            var product = await _productService.SearchArchiveProduct(productName);

            return Ok(new
            {
                messsage = product.Any() ?
                "Archive retrieved successfully"
                : "Archive not found",
                data = product
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("archive/{id}/restore")]
        public async Task<IActionResult> RestoreProduct(Guid id)
        {
            try
            {
                await _productService.RestoreArchiveInProduct(id);
                return Ok(new { message = "Product restored successfully" });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message });
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("archive")]
        public async Task<IActionResult> GetAllArchivesProduct()
        {
            var archives = await _productService.GetAllArchiveProducts();
            return Ok(new
            {
                message = archives.Any()
                    ? "Archive retrieved successfully"
                    : "Archives not found",
                data = archives
            });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("archive/{id}")]
        public async Task<IActionResult> DeleteArchiveProductById(Guid id)
        {
            try
            {
                await _productService.DeleteProductInArchive(id);
                return Ok(new { message = "Archive deleted successfully" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return NotFound(new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcelProduct(IFormFile file)
        {
            try
            {
                await _productService.ImportExcelProduct(file);
                return Ok(new { message = "Import product successfully" });
            }
            catch (System.Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

    }
}
