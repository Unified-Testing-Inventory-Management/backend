using OfficeOpenXml;
using Server.Data;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services
{
    public class ProductServices(CurrentUserServices currentUserServices, GenerateImageServices generateImageServices, IProductRepository productRepositories) : IProductService
    {
        private readonly CurrentUserServices _currentUserServices = currentUserServices;
        private readonly GenerateImageServices _generateImageServices = generateImageServices;
        private readonly IProductRepository _productRepositories = productRepositories;

        public async Task<List<Product>> GetAllProducts()
        {
            var userId = _currentUserServices.GetLoggedInUser();
            return await _productRepositories.GetAllProducts(userId);
        }

        public async Task CreateProduct(ProductDTOs.CreateProductDTOs _createProductDtOs)
        {
            var userId = _currentUserServices.GetLoggedInUser();

            if (string.IsNullOrWhiteSpace(_createProductDtOs.ProductName))
                throw new ArgumentException("Product name is required");

            if (_createProductDtOs.Price <= 0)
                throw new ProductExceptions.InvalidProductPriceException("Product price cannot be negative", 400);

            if (_createProductDtOs.StockQuantity < 0)
                throw new ProductExceptions.InvalidStockQuantityException("Product stock cannot be negative", 400);

            var productInArchive = await _productRepositories.GetProductNameInArchive(_createProductDtOs.ProductName, userId);
            var product = await _productRepositories.GetProductNameInProduct(_createProductDtOs.ProductName, userId);

            if (product is not null)
                throw new ProductExceptions.ProductAlreadyExists($"Product name {_createProductDtOs.ProductName} already exists", 409);

            if (productInArchive is not null)
                throw new ProductExceptions.ProductAlreadyExists($"Product name {_createProductDtOs.ProductName} already exists", 409);

            var imagePath = await _generateImageServices.Generate(_createProductDtOs);

            await _productRepositories.SaveProduct(userId, imagePath!, _createProductDtOs);
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var product = _productRepositories.GetProductById(userId, id) ?? throw new ProductExceptions.ProductNotFoundException($"Product Id {userId} not found", 404);
            return await product;
        }

        public async Task<List<Product>> SearchProduct(string productName)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            return await _productRepositories.SearchNameInProduct(userId, productName);
        }

        public async Task ArchiveProductById(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var product = await _productRepositories.GetProductById(userId, id) ?? throw new ProductExceptions.ProductNotFoundException($"Product Id {userId} not found", 404);
            await _productRepositories.SaveProductInArchive(userId, product);
        }

        public async Task<List<Archive>> SearchArchiveProduct(string productName)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            return await _productRepositories.SearchNameInArchive(userId, productName);
        }

        public async Task UpdateProductById(ProductDTOs.UpdateProductDTOs _updateProductDTOs, Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var product = await _productRepositories.GetProductById(userId, id) ?? throw new ProductExceptions.ProductNotFoundException($"Product Id {userId} not found", 404);

            if (_updateProductDTOs.Price < 0)
                throw new ProductExceptions.InvalidProductPriceException("Product price cannot be negative", 400);

            await _productRepositories.UpdateProduct(_updateProductDTOs, product);
        }

        public async Task<List<Archive>> GetAllArchiveProducts()
        {
            var userId = _currentUserServices.GetLoggedInUser();
            return await _productRepositories.GetAllArchiveProducts(userId);
        }

        public async Task RestoreArchiveInProduct(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchive = await _productRepositories.GetProductArchiveById(userId, id) ?? throw new ProductExceptions.ProductNotFoundException($"Product Id {userId} not found", 404);
            await _productRepositories.RestoreProductInArchive(userId, productArchive);
        }

        public async Task DeleteProductInArchive(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchive = await _productRepositories.GetProductArchiveById(userId, id) ?? throw new ProductExceptions.ProductNotFoundException($"Product Id {userId} not found", 404);
            await _productRepositories.DeleteProductInArchive(productArchive);
        }

        public async Task ImportExcelProduct(IFormFile file)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            if (file == null || file.Length == 0)
                throw new Exception("No file uploaded.");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension?.Rows ?? 0;

                    if (rowCount < 2)
                        throw new BadHttpRequestException("Excel file is empty or missing header.");

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var Id = Guid.NewGuid();
                        int price = 0;
                        int stockQuantity = 0;

                        string productName = worksheet.Cells[row, 1].Text;
                        string category = worksheet.Cells[row, 2].Text;
                        int.TryParse(worksheet.Cells[row, 3].Text, out price);
                        int.TryParse(worksheet.Cells[row, 4].Text, out stockQuantity);

                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        await _productRepositories.SaveProductFromExcelImport(userId, productName, category, price, stockQuantity);
                    }
                }
            }
        }
    }
}
