using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class TransactionServices : ITransactionInterface
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
            throw new ArgumentException(nameof(_createSale.ProductName),"Product transaction not found");

        if (productTransaction.StockQuantity >= _createSale.Quantity)
        {
            productTransaction.StockQuantity -= _createSale.Quantity;
        }
        else
        {
            throw new InvalidOperationException("Not enough stock available");
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
        return await _db.Sales
            .Include(s => s.SaleDetails)
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