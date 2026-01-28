using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Interface;

namespace Server.Controllers
{
    [Route("api/v1/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
    private readonly ITransactionInterface _transactionServices;
    
    public TransactionController(ITransactionInterface transactionInterface)
    {
        _transactionServices = transactionInterface;
    }
    
    [Authorize(Policy = "OwnerOnly")]
    [HttpPost]
    public async Task<IActionResult> CreateSale(TransactionDTOs.CreateProductSale _createSale)
    {
        try
        {
            await _transactionServices.CreateProductSale(_createSale);
            return Ok(new {message = "Transaction Successfully"});
        }
        catch (Exception e)
        {
            return NotFound( new { error = e.Message } );
        }
    }

    [Authorize(Policy = "OwnerOnly")]
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
            return NotFound(new {error = e.Message});
        }
    }

    [Authorize(Policy = "OwnerOnly")]
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
            return NotFound(new {error = e.Message});
        }
    }


    }
}
