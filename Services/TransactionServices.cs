using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class TransactionServices : ITransactionService
{
    private readonly AppDbContext _db;
    private readonly CurrentUserServices _currentUserServices;
    
    public TransactionServices(AppDbContext appDb, CurrentUserServices currentUserServices)
    {
        _db = appDb;
        _currentUserServices = currentUserServices;
    }
    
    public async Task CreateProductSale(TransactionDTOs.CreateProductSale _createSale)
    {
        var userId = _currentUserServices.GetLoggedInUser();
        var productTransaction = await _db.Products.FirstOrDefaultAsync(p => p.UserId == userId && p.Id == _createSale.ProductId);

        if (productTransaction == null)
            throw new TransactionExceptions.ProductInTrasactionNotFoundException(_createSale.ProductName);
        
        if (productTransaction.StockQuantity >= _createSale.Quantity)
        {
            productTransaction.StockQuantity -= _createSale.Quantity;
        }
        else
        {
            throw new TransactionExceptions.NotEnoughStockException(_createSale.ProductName);
        }

        var saleId = Guid.NewGuid();
        var saleDetailId = Guid.NewGuid();
        var totalAmount = (_createSale.Quantity * _createSale.Price);
        
        var saveProductSale = new Sale
        {
            Id = saleId,
            ProductId =_createSale.ProductId,
            UserId = userId,
            SaleDate = DateTime.Now,
            TotalAmount = totalAmount
        };

        var saleProductDetails = new Sale_Detail
        {
            Id = saleDetailId,
            ProductName = _createSale.ProductName,
            Category = _createSale.Category,
            ProductId = saveProductSale.ProductId,
            SaleId = saveProductSale.Id,
            Price = _createSale.Price,
            Quantity = _createSale.Quantity,
        };
        
        _db.Sales.Add(saveProductSale);
        _db.Sale_Details.Add(saleProductDetails);
        
        await _db.SaveChangesAsync();
    }

    public async Task<List<Sale>> AllProductTransaction()
    {
        var  userId = _currentUserServices.GetLoggedInUser();
        return await _db.Sales
            .Include(s => s.SaleDetails).Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> SearchSaleProductTransaction(string productName)
    {
        var userId = _currentUserServices.GetLoggedInUser();
        var query = _db.Sales.Where(sp => sp.UserId == userId).AsQueryable();
        
        if (!string.IsNullOrEmpty(productName))
        {
           query = query.Where(p  => p.SaleDetails.Any(s => s.ProductName == productName));
        }
        
        return await query.ToListAsync();
    }
}