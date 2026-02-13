using System;
using System.ComponentModel.DataAnnotations;
using Server.Models;

namespace Server.DTOs;

public class ProductDTOs
{
    public class CreateProductDTOs
    {
        public required string ProductName { get; set; }
        public required string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class UpdateProductDTOs
    {
        public required string ProductName { get; set; }
        public required string Category { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Price cannot be negative")]
        public required int Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public required int StockQuantity { get; set; }
    }

    public class StockInsightsDTOs
    {
        public Guid? ProductId { get; set; }
        public Guid? UserId { get; set; }
        public required string ProductName { get; set; }
        public int StockQuantity { get; set; }
        public required string Status { get; set; }
        public required string Message { get; set; }
        public DateTime? SaleDate { get; set; }
    }
}
