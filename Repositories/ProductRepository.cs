using System;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Repositories;

public class ProductRepository(AppDbContext appDb) : IProductRepository
{
    private readonly AppDbContext _db = appDb;

    public async Task<List<Product>> GetAllProducts(Guid userId)
    {
        return await _db.Products.Where(p => p.UserId == userId).OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Product?> GetProductById(Guid userId, Guid id)
    {
        return await _db.Products.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
    }

    public async Task<Archive?> GetProductArchiveById(Guid userId, Guid id)
    {
        return await _db.Archives.FirstOrDefaultAsync(a => a.UserId == userId && a.ProductId == id);
    }

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

    public async Task<Product?> GetProductNameInProduct(string productName, Guid userId)
    {
        return await _db.Products.FirstOrDefaultAsync(p => p.ProductName == productName && p.UserId == userId);
    }

    public async Task<Archive?> GetProductNameInArchive(string productName, Guid userId)
    {
        return await _db.Archives.FirstOrDefaultAsync(p => p.ProductName == productName && p.UserId == userId);
    }

    public async Task SaveProduct(Product product)
    {
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

    public async Task<List<Archive>> GetAllArchiveProducts(Guid userId)
    {
        return await _db.Archives.Where(a => a.UserId == userId).OrderByDescending(a => a.DeletedAt).ToListAsync();
    }

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
}
