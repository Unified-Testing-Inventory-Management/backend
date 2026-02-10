using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class TransactionServices(ITransactionRepository transactionRepository, CurrentUserServices currentUserServices) : ITransactionService
{
    private readonly CurrentUserServices _currentUserServices = currentUserServices;
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    public async Task CreateProductSale(TransactionDTOs.CreateProductSale _createSale)
    {
        var userId = _currentUserServices.GetLoggedInUser();
        var productTransaction = await _transactionRepository.GetProductById(userId, _createSale.ProductId) ?? throw new TransactionExceptions.ProductInTransactionNotFoundException($"Product {_createSale.ProductName} not found for transaction", 404);

        if (productTransaction.StockQuantity >= _createSale.Quantity)
        {
            productTransaction.StockQuantity -= _createSale.Quantity;
        }
        else
        {
            throw new TransactionExceptions.NotEnoughStockException($"Not enough stock in transaction product {_createSale.ProductName}", 400);
        }

        var totalAmount = _createSale.Quantity * _createSale.Price;

        var saveProductSale = await _transactionRepository.SaveProductInSale(userId, totalAmount, _createSale.ProductId);

        await _transactionRepository.SaveProductInSaleDetails(saveProductSale.Id, saveProductSale.ProductId, _createSale);

    }
    public async Task<List<Sale>> AllProductTransaction()
    {
        var userId = _currentUserServices.GetLoggedInUser();
        return await _transactionRepository.AllProductTransactions(userId);
    }
    public async Task<List<Sale>> SearchSaleProductTransaction(string productName)
    {
        var userId = _currentUserServices.GetLoggedInUser();
        var query = _transactionRepository.SearchSaleProductTransaction(userId, productName);
        return await query;
    }
}