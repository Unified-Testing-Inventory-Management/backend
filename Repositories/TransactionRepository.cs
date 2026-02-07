using System;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Repositories;

public class TransactionRepository(AppDbContext appDb) : ITransactionRepository
{
    private readonly AppDbContext _db = appDb;

    public async Task<List<Sale>> AllProductTransactions(Guid userId)
    {
        return await _db.Sales
            .Include(s => s.SaleDetails).Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<Product?> GetProductById(Guid userId, Guid id)
    {
        return await _db.Products.FirstOrDefaultAsync(p => p.UserId == userId && p.Id == id);
    }

    public async Task<Sale> SaveProductInSale(Guid userId, Guid saleId, int totalAmount, Guid productId)
    {
        var saveProductInSale = new Sale
        {
            Id = saleId,
            ProductId = productId,
            UserId = userId,
            SaleDate = DateTime.Now,
            TotalAmount = totalAmount
        };

        _db.Sales.Add(saveProductInSale);
        await _db.SaveChangesAsync();

        return saveProductInSale;
    }

    public async Task SaveProductInSaleDetails(Guid saleDetailId, Guid saleId, Guid productId, TransactionDTOs.CreateProductSale createProductSale)
    {
        var saleProductDetails = new Sale_Detail
        {
            Id = saleDetailId,
            ProductName = createProductSale.ProductName,
            Category = createProductSale.Category,
            ProductId = productId,
            SaleId = saleId,
            Price = createProductSale.Price,
            Quantity = createProductSale.Quantity,
        };

        _db.Sale_Details.Add(saleProductDetails);

        await _db.SaveChangesAsync();
    }

    public async Task<List<Sale>> SearchSaleProductTransaction(Guid userId, string productName)
    {
        var query = _db.Sales.Where(sp => sp.UserId == userId).AsQueryable();

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(p => p.SaleDetails.Any(s => s.ProductName == productName));
        }

        return await query.ToListAsync();
    }
}
