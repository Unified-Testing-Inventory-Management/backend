using System;

namespace Server.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; } // Admin
    public DateOnly CreatedAt { get; set; }

    public ICollection<Product> Products {get;set;} = new List<Product>();
}
