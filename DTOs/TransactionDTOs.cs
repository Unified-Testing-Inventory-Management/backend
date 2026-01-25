using Server.Models;

namespace Server.DTOs;

public class TransactionDTOs
{
    public class CreateProductSale
    {
    public Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string Category { get; set; }
    public Guid? UserId { get; set; }
    public int TotalAmount { get; set; }
    public int Quantity { get; set; }
    public int Price { get; set; }
    }
}