namespace Server.Exceptions;

public class UserExceptions
{
    public class UserAlreadyExists(string message, int status) : Exception
    {

        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }

    public class UnAuthorizedUserException : Exception
    {
        override
        public string Message
        { get; }
        public int StatusCode { get; set; }
        public UnAuthorizedUserException(string message, int status)
        {
            Message = message;
            StatusCode = status;
        }
    }

    public class UserNotFoundException : Exception
    {
        override
        public string Message
        { get; }
        public int StatusCode { get; set; }
        public UserNotFoundException(string message, int status)
        {
            Message = message;
            StatusCode = status;
        }
    }
}