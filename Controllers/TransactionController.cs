using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Server.DTOs;
using Server.Interface;
using Server.Exceptions;

namespace Server.Controllers
{
    [Route("api/v1/transactions")]
    [ApiController]
    [EnableRateLimiting("api")]
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
                return Ok(new { message = "Transaction Successfully", status = 201 });
            }
            catch (TransactionExceptions.ProductInTransactionNotFoundException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return BadRequest(new { error = e.Message, status = e.StatusCode });
            }
            catch (TransactionExceptions.NotEnoughStockException e)
            {
                Console.WriteLine(new { error = e.Message, status = e.StatusCode });
                return BadRequest(new { error = e.Message, status = e.StatusCode });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<IActionResult> GetAllProductSale()
        {
            try
            {
                var saleProduct = await _transactionServices.AllProductTransaction();
                return Ok(new { message = "Transaction Successfully", status = 200, data = saleProduct });
            }
            catch (Exception e)
            {
                Console.WriteLine(new { error = e.Message, status = 500 });
                return StatusCode(500, new { error = e.Message });
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchSaleProductTransaction(string productName)
        {
            try
            {
                var salesProduct = await _transactionServices.SearchSaleProductTransaction(productName);

                Console.WriteLine(new
                {
                    message = salesProduct.Any() ? "Sale Product Successfully" :
                                                    "Sale Product Not Found",
                    status = 200,
                    data = salesProduct
                });

                return Ok(new
                {
                    message = salesProduct.Any() ? "Sale Product Successfully" :
                                                    "Sale Product Not Found",
                    status = 200,
                    data = salesProduct
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(new { error = e.Message, status = 500 });
                return StatusCode(500, new { error = e.Message });
            }
        }


    }
}
