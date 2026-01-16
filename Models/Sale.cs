using System;

namespace Server.Models;

public class Sale
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public User? User;
    public DateOnly SaleDate { get; set; }
    public float TotalAmount { get; set; }
}
