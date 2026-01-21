using System;

namespace Server.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Image { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? BarCode { get; set; }
    public DateOnly CreatedAt { get; set; }

    public Guid? UserId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User? User { get; set; }

}
