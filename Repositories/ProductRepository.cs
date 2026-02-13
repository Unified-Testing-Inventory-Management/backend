using System;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;
using Server.Services;

namespace Server.Repositories;

public class ProductRepository(AppDbContext appDb, BarCodeServices barCodeServices) : IProductRepository
{
    private readonly AppDbContext _db = appDb;
    private readonly BarCodeServices _barCodeServices = barCodeServices;

    public async Task<List<Product>> GetAllProducts(Guid userId) =>
        await _db.Products.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();

    public async Task<Product?> GetProductById(Guid userId, Guid id) =>
        await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

    public async Task<Archive?> GetProductArchiveById(Guid userId, Guid id) =>
        await _db.Archives.FirstOrDefaultAsync(a => a.UserId == userId && a.ProductId == id);

    public async Task<List<Product>> SearchNameInProduct(Guid userId, string productName)
    {
        var query = _db.Products.Where(u => u.UserId == userId).AsQueryable();

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(p => EF.Functions.Like(p.ProductName, $"%{productName}%"));
        }

        return await query.ToListAsync();
    }

    public async Task<List<Archive>> SearchNameInArchive(Guid userId, string productName)
    {
        var query = _db.Archives.Where(ap => ap.UserId == userId).AsQueryable();

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(ap => EF.Functions.Like(ap.ProductName, $"%{productName}%"));
        }

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductNameInProduct(string productName, Guid userId) =>
        await _db.Products.FirstOrDefaultAsync(p => p.ProductName == productName && p.UserId == userId);

    public async Task<Archive?> GetProductNameInArchive(string productName, Guid userId) =>
        await _db.Archives.FirstOrDefaultAsync(p => p.ProductName == productName && p.UserId == userId);

    public async Task SaveProduct(Guid userId, string imagePath, ProductDTOs.CreateProductDTOs _createProductDTOs)
    {
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            UserId = userId,
            ProductName = char.ToUpper(_createProductDTOs.ProductName[0]) + _createProductDTOs.ProductName.Substring(1).ToLower(),
            Category = char.ToUpper(_createProductDTOs.Category[0]) + _createProductDTOs.Category.Substring(1).ToLower(),
            Price = _createProductDTOs.Price,
            StockQuantity = _createProductDTOs.StockQuantity,
            Image = imagePath,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            BarCode = _barCodeServices.GenerateProductBarcode(productId)
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task SaveProductInArchive(Guid userId, Product product)
    {
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
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            DeletedAt = DateTime.Now
        };

        _db.Archives.Add(productSaveInArchive);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateProduct(ProductDTOs.UpdateProductDTOs updateProductDTOs, Product product)
    {
        product.ProductName = updateProductDTOs.ProductName;
        product.Category = updateProductDTOs.Category;
        product.Price = updateProductDTOs.Price;
        product.StockQuantity = updateProductDTOs.StockQuantity;
        product.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
    }

    public async Task<List<Archive>> GetAllArchiveProducts(Guid userId) =>
        await _db.Archives.Where(a => a.UserId == userId).OrderByDescending(a => a.DeletedAt).ToListAsync();

    public async Task RestoreProductInArchive(Guid userId, Archive archive)
    {
        _db.Archives.Remove(archive);

        var restoreProduct = new Product
        {
            Id = archive.ProductId,
            UserId = archive.UserId,
            ProductName = archive.ProductName,
            Category = archive.Category,
            Price = archive.Price,
            StockQuantity = archive.StockQuantity,
            BarCode = archive.BarCode,
            CreatedAt = archive.CreatedAt,
            UpdatedAt = archive.UpdatedAt
        };

        _db.Products.Add(restoreProduct);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteProductInArchive(Archive archive)
    {
        _db.Archives.Remove(archive);
        await _db.SaveChangesAsync();
    }

    public async Task SaveProductFromExcelImport(Guid userId, string productName, string category, int price, int stockQuantity)
    {
        var Id = Guid.NewGuid();
        var barCode = _barCodeServices.GenerateProductBarcode(Id);

        var product = new Product
        {
            Id = Id,
            UserId = userId,
            ProductName = productName,
            Category = category,
            Price = price,
            StockQuantity = stockQuantity,
            BarCode = barCode,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ProductDTOs.StockInsightsDTOs>> StockInsights(Guid userId)
    {
        var oneMonthAgo = DateTime.UtcNow.AddDays(-30);

        var stockInsights = await _db.Products
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.ProductName,
                p.StockQuantity,

                LastSaleDate = _db.Sales
                    .Where(s => s.ProductId == p.Id)
                    .Max(s => (DateTime?)s.SaleDate)
            })
            .Select(p => new ProductDTOs.StockInsightsDTOs
            {
                ProductId = p.Id,
                UserId = userId,
                ProductName = p.ProductName!,
                StockQuantity = p.StockQuantity,

                SaleDate = p.LastSaleDate ?? DateTime.MinValue,

                Status =
                    p.StockQuantity == 0
                        ? "OUT OF STOCK"
                    : p.StockQuantity <= 5
                        ? "LOW STOCK"
                    : p.LastSaleDate == null
                        ? "NO SALES YET"
                    : p.LastSaleDate < oneMonthAgo
                        ? "NO SALES IN 30 DAYS"
                    : "IN STOCK",

                Message =
                    p.StockQuantity == 0
                        ? $"{p.ProductName} is OUT OF STOCK"
                    : p.StockQuantity <= 5
                        ? $"{p.ProductName} is running LOW STOCK"
                    : p.LastSaleDate == null
                        ? $"{p.ProductName} has never been sold"
                    : p.LastSaleDate < oneMonthAgo
                        ? $"{p.ProductName} has not been sold in the last 30 days"
                    : $"{p.ProductName} has sufficient STOCK"
            })
            .Where(p => p.Status != "IN STOCK")
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();

        return stockInsights;
    }

}
