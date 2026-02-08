using System;
using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface ITransactionRepository
{
    Task<Product?> GetProductById(Guid userId, Guid id);
    Task<Sale> SaveProductInSale(Guid userId, int totalAmount, Guid productId);
    Task SaveProductInSaleDetails(Guid saleId, Guid productId, TransactionDTOs.CreateProductSale createProductSale);
    Task<List<Sale>> AllProductTransactions(Guid userId);
    Task<List<Sale>> SearchSaleProductTransaction(Guid userId,string productName);
}
