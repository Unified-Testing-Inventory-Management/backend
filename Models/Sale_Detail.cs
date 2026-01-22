using System;

namespace Server.Models;

public class Sale_Detail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? ProductName { get; set; }
    public Guid SaleId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Sale? Sale;
    public Guid ProductId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public Product? Product;
    public int Quantity { get; set; }
    public float Price { get; set; }
}
