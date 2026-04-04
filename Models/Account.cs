using System;

namespace Server.Models
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? AccountName { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public User? User { get; set; }

        public string? Role { get; set; }
        public DateTime DateTime { get; set; }
    }
}