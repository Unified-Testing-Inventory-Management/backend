using System;

namespace Server.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public float Price { get; set; }
    public int StockQuantity { get; set; }
    public int LowStockLevel { get; set; }
    public DateOnly CreatedAt { get; set; }

}
