using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Services
{
    public class ProductServices : IProductInterface
    {
        private readonly AppDbContext _db;
        private CurrentUserServices _currentUserServices;
        private BarCodeServices _barCodeServices;

        public ProductServices(AppDbContext db,CurrentUserServices currentUserServices, BarCodeServices barCodeServices)
        {
            _db = db;
            _currentUserServices = currentUserServices;
            _barCodeServices = barCodeServices;
        }


        public async Task<List<Product>> GetAllProducts()
        {
            var userId = _currentUserServices.GetLoggedInUser();
            return await _db.Products.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task CreateProduct(ProductDTOs.CreateProductDTOs _createProductDtOs)
        {
            var userId = _currentUserServices.GetLoggedInUser();

            if (string.IsNullOrWhiteSpace(_createProductDtOs.ProductName))
                throw new ArgumentException("Product name is required");

            if (_createProductDtOs.Price <= 0)
                throw new ArgumentException(nameof(_createProductDtOs.Price),"Price must be greater than zero");

            if (_createProductDtOs.StockQuantity < 0)
                throw new ArgumentException(nameof(_createProductDtOs.StockQuantity), "Stock quantity cannot be negative");

            var productExists = await _db.Products.AnyAsync(p =>
                p.ProductName == _createProductDtOs.ProductName &&
                p.UserId == userId
            );

            if (productExists)
                throw new ArgumentException(nameof(_createProductDtOs.ProductName),"Product already registered");

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
                CreatedAt = DateTime.Now
            };

            product.BarCode = _barCodeServices.GenerateProductBarcode(productId);

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }


        public async Task<Product> GetProductById(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();

            var product = await _db.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (product == null)
            {
                throw new ArgumentOutOfRangeException(nameof(product.ProductName), "Product not found");
            }

            return product;
        }

        public async Task<List<Product>> SearchProduct(string productName)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var query = _db.Products.Where(u => u.UserId == userId).AsQueryable();
            
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));
            }

            return await query.ToListAsync();
        }

        public async Task ArchiveProductById(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var product = await _db.Products.FirstOrDefaultAsync(p => p.UserId == userId && p.Id == id);

            if (product == null)
            {
                throw new ArgumentException(nameof(product.ProductName),
                    "Product Not found");
            }

            _db.Products.Remove(product);


            var Id = Guid.NewGuid();
            var productSaveInArchive = new Archive
            {
                Id = Id,
                ProductId = product.Id,
                UserId = userId,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                BarCode = product.BarCode,
                DeletedAt = DateTime.Now
            };

            _db.Archives.Add(productSaveInArchive);
            await _db.SaveChangesAsync();
        }

        public Task<List<Archive>> SearchArchiveProduct(string productName)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var query = _db.Archives.Where(ap => ap.UserId == userId).AsQueryable();
            
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(ap => EF.Functions.Like(ap.ProductName, $"%{productName}%"));
            }

            return query.ToListAsync();
        }

        public async Task UpdateProductById(ProductDTOs.UpdateProductDTOs _updateProductDTOs, Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var product = await _db.Products.FirstOrDefaultAsync(p => p.UserId == userId && p.Id == id);

            if (product == null)
            {
                throw new ArgumentException(nameof(_updateProductDTOs.ProductName),
                     "Product Not found");
            }

            if (_updateProductDTOs.Price < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(_updateProductDTOs.Price),
                    "Price cannot be negative");
            }

            product.ProductName = _updateProductDTOs.ProductName;
            product.Category = _updateProductDTOs.Category;
            product.Price = _updateProductDTOs.Price;
            product.StockQuantity = _updateProductDTOs.StockQuantity;

            await _db.SaveChangesAsync();

        }
        
        public async Task<List<Archive>> GetAllArchivesProduct()
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchives = await _db.Archives.Where(a => a.UserId == userId).OrderByDescending(a => a.DeletedAt).ToListAsync();
            return productArchives;
        }

        public async Task RestoreArchiveProduct(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchive = await _db.Archives.FirstOrDefaultAsync(a => a.UserId == userId && a.ProductId == id);
            if (productArchive == null)
            {
                throw new ArgumentException(nameof(productArchive.ProductName), "Archive not found");
            }
            _db.Archives.Remove(productArchive);
            
            var restoreProduct = new Product
            {
                Id = productArchive.ProductId,
                UserId = productArchive.UserId,
                ProductName = productArchive.ProductName,
                Category = productArchive.Category,
                Price = productArchive.Price,
                StockQuantity = productArchive.StockQuantity,
                BarCode = productArchive.BarCode,
                CreatedAt = productArchive.DeletedAt
            };
            
            _db.Products.Add(restoreProduct);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteArchiveProduct(Guid id)
        {
            var userId = _currentUserServices.GetLoggedInUser();
            var productArchive = await _db.Archives.FirstOrDefaultAsync(a => a.UserId == userId && a.ProductId == id);
            if (productArchive == null)
            {
                throw new ArgumentException(nameof(productArchive.ProductName), "Archive not found");
            }
            _db.Archives.Remove(productArchive);
            
            await _db.SaveChangesAsync();
        }
    }
}
