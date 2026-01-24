using System;

namespace Server.Models;

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public Product? Product;
    public Guid? UserId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public User? User;

    public DateTime SaleDate { get; set; }
    public int TotalAmount { get; set; }
    public ICollection<Sale_Detail> SaleDetails { get; set; } = new  List<Sale_Detail>();
}
