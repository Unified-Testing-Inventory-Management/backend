namespace Server.Exceptions;

public class UserExceptions
{
    public class UserAlreadyExists : Exception
    {
        public string Username { get; set; }

        public UserAlreadyExists(string username) : base($"{username} is already exists")
        {
            Username = username;
        }
    }

    public class UnAuthorizedUserException: Exception
    {
        public UnAuthorizedUserException() : base("Username or password is incorrect") { }
    }
}