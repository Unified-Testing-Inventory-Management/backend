namespace Server.Exceptions;

public class ProductExceptions
{
    public class ProductAlreadyExists(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }

    public class ProductNotFoundException(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }

    public class InvalidProductPriceException(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }

    public class InvalidStockQuantityException(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }
}