using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface ITransactionService
{
    Task CreateProductSale(TransactionDTOs.CreateProductSale _createSale);
    Task<List<Sale>> AllProductTransaction();
    Task<List<Sale>> SearchSaleProductTransaction(string productName);
}