using System;

namespace Server.Models;

public class Account()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? AccountName { get; set; }
    public string? Role { get; set; }
    public DateOnly DateOnly { get; set; }
}
