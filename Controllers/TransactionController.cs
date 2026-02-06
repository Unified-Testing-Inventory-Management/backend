using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Interface;
using Server.Exceptions;

namespace Server.Controllers
{
    [Route("api/v1/transactions")]
    [ApiController]
    public class TransactionController(ITransactionService transactionInterface) : ControllerBase
    {
    private readonly ITransactionService _transactionServices = transactionInterface;

    [Authorize(Policy = "AdminPolicy")]
    [HttpPost]
    public async Task<IActionResult> CreateSale(TransactionDTOs.CreateProductSale _createSale)
    {
        try
        {
            await _transactionServices.CreateProductSale(_createSale);
            return Ok(new {message = "Transaction Successfully"});
        }
        catch (TransactionExceptions.ProductInTrasactionNotFoundException e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(404, new { error = e.Message });
        }
        catch (TransactionExceptions.NotEnoughStockException e)
        {
            Console.WriteLine(e.Message);
            return StatusCode(402, new { error = e.Message } );
        }
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet]
    public async Task<IActionResult> GetAllProductSale()
    {
        try
        {
            var saleProduct = await _transactionServices.AllProductTransaction();
            return Ok(new {message = "Transaction Successfully",data = saleProduct});
        }
        catch (Exception e)
        {
            return StatusCode(500,new {error = e.Message});
        }
    }

    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("search")]
    public async Task<IActionResult> SearchSaleProductTransaction(string productName)
    {
        try
        {
            var salesProduct = await _transactionServices.SearchSaleProductTransaction(productName);
            return Ok(new
            {
                message = salesProduct.Any() ? "Sale Product Successfully" :
                                                "Sale Product Not Found",
                data = salesProduct
            });
        }
        catch (Exception e)
        {
            return StatusCode(500,new {error = e.Message});
        }
    }


    }
}
