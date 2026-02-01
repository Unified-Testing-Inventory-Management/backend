namespace Server.Exceptions;

public class TransactionExceptions
{
    public class ProductInTrasactionNotFoundException : Exception
    {
        public string ProductName { get; }

        public ProductInTrasactionNotFoundException(string productName): base($"Product {productName} in transaction not found")
        {
            ProductName = productName;
        }
    }

    public class NotEnoughStockException : Exception
    {
        private string ProductName { get; }

        public NotEnoughStockException(string productName) : base($"Not enough stock in transaction {productName}")
        {
            ProductName = productName;
        }
    }
}