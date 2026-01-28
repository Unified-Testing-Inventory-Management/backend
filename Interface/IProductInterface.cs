using System;
using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface IProductInterface
{
    Task<List<Product>> GetAllProducts();
    Task CreateProduct(ProductDTOs.CreateProductDTOs _createProductDTOs);
    Task<Product> GetProductById(Guid id);
    Task<List<Product>> SearchProduct(string productName);
    Task UpdateProductById(ProductDTOs.UpdateProductDTOs _upddateProductDTOs, Guid id);
    Task ArchiveProductById(Guid id);
}
