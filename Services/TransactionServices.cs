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
        {
            throw new ArgumentException("Product transaction not found");
        }

        var updateProductStock = (_createSale.Quantity - productTransaction.StockQuantity);
        productTransaction.StockQuantity = updateProductStock;
        

        var saleId = Guid.NewGuid();
        var saleDetailId = Guid.NewGuid();
        var totalAmount = (_createSale.Quantity * _createSale.Price);
        
        var saveProductSale = new Sale
        {
            Id = saleId,
            ProductId =_createSale.ProductId,
            UserId = userId,
            SaleDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TotalAmount = totalAmount
        };

        var saleProductDetails = new Sale_Detail
        {
            Id = saleDetailId,
            ProductName = _createSale.ProductName,
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
}