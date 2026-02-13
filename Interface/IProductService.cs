using System;
using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface IProductService
{
    Task ImportExcelProduct(IFormFile file);
    Task<List<Product>> GetAllProducts();
    Task CreateProduct(ProductDTOs.CreateProductDTOs _createProductDTOs);
    Task<Product?> GetProductById(Guid id);
    Task<List<Product>> SearchProduct(string productName);
    Task UpdateProductById(ProductDTOs.UpdateProductDTOs _upddateProductDTOs, Guid id);
    Task ArchiveProductById(Guid id);
    Task<List<Archive>> SearchArchiveProduct(string productName);
    Task<List<Archive>>  GetAllArchiveProducts();
    Task RestoreArchiveInProduct(Guid id);
    Task DeleteProductInArchive(Guid id);
    Task<List<ProductDTOs.StockInsightsDTOs>> StockInsights();
}
