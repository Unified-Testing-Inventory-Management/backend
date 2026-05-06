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
    }

    public class LoginUserDTOs
    {
        public required string Identifier { get; set; }
        public required string Password { get; set; }

    }

    public class UpdateUserDTOs
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
    }
}
