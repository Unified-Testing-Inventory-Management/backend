using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;

namespace Server.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    [EnableRateLimiting("api")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();

            Console.WriteLine(new { message = "Products retrieved successfully", status = 200, data = products });
            return Ok(new { message = "Products retrieved successfully", status = 200, data = products });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductById(id);
                return Ok(new { message = "Product retrieved successfully", status = 200, data = product });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
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
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return BadRequest(new { error = e.Message, status = e.StatusCode });
            }
            catch (ProductExceptions.InvalidStockQuantityException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return BadRequest(new { error = e.Message, status = e.StatusCode });
            }
            catch (ProductExceptions.ProductAlreadyExists e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return Conflict(new { error = e.Message, status = e.StatusCode });
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
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
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

                Console.WriteLine(new { message = "Product archived successfully", status = 201 });
                return Ok(new { message = "Product archived successfully", status = 201 });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
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
                return Ok(new { message = "Product restored successfully", status = 201 });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("archive")]
        public async Task<IActionResult> GetAllArchivesProduct()
        {
            var archives = await _productService.GetAllArchiveProducts();

            Console.WriteLine(new
            {
                message = archives.Any()
                    ? "Archive retrieved successfully"
                    : "Archives not found",
                status = 200,
                data = archives
            });
            return Ok(new
            {
                message = archives.Any()
                    ? "Archive retrieved successfully"
                    : "Archives not found",
                status = 200,
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
                return Ok(new { message = "Archive deleted successfully", status = 201 });
            }
            catch (ProductExceptions.ProductNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return NotFound(new { error = e.Message, status = e.StatusCode });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportExcelProduct(IFormFile file)
        {
            try
            {                                                                                                                                                           
                await _productService.ImportExcelProduct(file);
                return Ok(new { message = "Import product successfully", status = 201 });
            }
            catch (Exception e)
            {
                Console.WriteLine(new { error = e.Message, status = 404 });
                return BadRequest(new { error = e.Message, status = 404 });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("product-insights")]
        public async Task<IActionResult> ProductInsights()
        {
            var productStocks = await _productService.StockInsights();
            Console.WriteLine(new { message = "Retrieved product in low stocks", data = productStocks });
            return Ok(new { message = "Retrieved all product stocks", data = productStocks});
        }

    }
}
