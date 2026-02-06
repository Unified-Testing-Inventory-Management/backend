using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services
{
    public class ProductServices(CurrentUserServices currentUserServices, BarCodeServices barCodeServices, IProductRepository productRepositories) : IProductService
    {
        private readonly CurrentUserServices _currentUserServices = currentUserServices;
        private readonly BarCodeServices _barCodeServices = barCodeServices;
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
                throw new ProductExceptions.InvalidProductPriceException();

            if (_createProductDtOs.StockQuantity < 0)
                throw new ProductExceptions.InvalidStockQuantityException();

            var productInArchive = await _productRepositories.GetProductNameInArchive(_createProductDtOs.ProductName, userId);

            var productExists = await _productRepositories.GetProductNameInProduct(_createProductDtOs.ProductName, userId);


            if (productExists != null)
                throw new ProductExceptions.ProductAlreadyExists(productExists.ProductName!);

            if (productInArchive != null)
                throw new ProductExceptions.ProductAlreadyExists(productInArchive.ProductName!);

            var productId = Guid.NewGuid();
            string? imagePath = null;

            if (_createProductDtOs.Image != null)
            {
                if (!_createProductDtOs.Image.ContentType.StartsWith("image/"))
                    throw new ArgumentException("Invalid image format");

                if (_createProductDtOs.Image.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("Image size cannot exceed 5MB");

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "products"
                );

                Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(_createProductDtOs.Image.FileName);
                var fileName = $"{productId}{extension}";
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await _createProductDtOs.Image.CopyToAsync(stream);
                }

                imagePath = $"/products/{fileName}";
            }

            var product = new Product
            {
                Id = productId,
                UserId = userId,
                ProductName = char.ToUpper(_createProductDtOs.ProductName[0]) + _createProductDtOs.ProductName.Substring(1).ToLower(),
                Category = char.ToUpper(_createProductDtOs.Category[0]) + _createProductDtOs.Category.Substring(1).ToLower(),
                Price = _createProductDtOs.Price,
                StockQuantity = _createProductDtOs.StockQuantity,
                Image = imagePath,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            product.BarCode = _barCodeServices.GenerateProductBarcode(productId);

            await _productRepositories.SaveProduct(product);
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();

            var product = _productRepositories.GetProductById(userId, id);

            if (product == null)
                throw new ProductExceptions.ProductNotFoundException(id);

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
            var product = await _productRepositories.GetProductById(userId, id);

            if (product == null)
                throw new ProductExceptions.ProductNotFoundException(id);

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
            var product = await _productRepositories.GetProductById(userId, id);

            if (product == null)
                throw new ProductExceptions.ProductNotFoundException(id);

            if (_updateProductDTOs.Price < 0)
                throw new ProductExceptions.InvalidProductPriceException();

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
            var productArchive = await _productRepositories.GetProductArchiveById(userId, id);

            if (productArchive == null)
                throw new ProductExceptions.ProductNotFoundException(id);

            await _productRepositories.RestoreProductInArchive(userId, productArchive);
        }

        public async Task DeleteProductInArchive(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchive = await _productRepositories.GetProductArchiveById(userId, id);

            if (productArchive == null)
                throw new ProductExceptions.ProductNotFoundException(id);

            await _productRepositories.DeleteProductInArchive(productArchive);
        }
    }
}
