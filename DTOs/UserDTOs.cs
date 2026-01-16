using System;

namespace Server.DTOs;

public class UserDTOs
{
    public class RegisterUserDTOs
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // Owner
    }

    public class LoginUserDTOs
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

    }
}
