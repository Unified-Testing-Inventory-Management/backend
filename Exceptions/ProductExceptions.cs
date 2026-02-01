namespace Server.Exceptions;

public class ProductExceptions
{
    public class ProductAlreadyExists : Exception
    {
        public string ProductName { get;}
        public ProductAlreadyExists(string productName):base($"Product '{productName}' already exists")
        {
            ProductName = productName;
        }
    }
    
    public class ProductNotFoundException : Exception
    {
        public Guid ProductId { get; }
        public ProductNotFoundException(Guid productId) : base($"Product id '{productId}' not found")
        {
            ProductId = productId;
        }
    }

    public class InvalidProductPriceException : Exception
    {
        public InvalidProductPriceException() : base($"Price must be greater than zero") { }
    }

    public class InvalidStockQuantityException : Exception
    {
        public InvalidStockQuantityException() : base("Stock quantity cannot be negative") { }
    }
}