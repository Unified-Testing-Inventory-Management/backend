namespace Server.Exceptions;

public class TransactionExceptions
{
    public class ProductInTransactionNotFoundException(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }

    public class NotEnoughStockException(string message, int status) : Exception
    {
        override
        public string Message
        { get; } = message;
        public int StatusCode { get; set; } = status;
    }
}