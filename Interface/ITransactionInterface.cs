using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface ITransactionInterface
{
    Task CreateProductSale(TransactionDTOs.CreateProductSale _createSale);
    Task<List<Sale>> AllProductTransaction();
}