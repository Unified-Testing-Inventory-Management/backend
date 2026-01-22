using Server.Models;

namespace Server.DTOs;

public class TransactionDTOs
{
    public class CreateProductSale
    {
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public Guid? UserId { get; set; }
    public float TotalAmount { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    }
}