using System;
using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface IProductRepository
{
    Task<List<Product>> GetAllProducts(Guid userId);
    Task<Product?> GetProductById(Guid userId, Guid id);
    Task<Archive?> GetProductArchiveById(Guid userId, Guid id);
    Task<List<Product>> SearchNameInProduct(Guid userId, string productName);
    Task<List<Archive>> SearchNameInArchive(Guid userId, string productName);
    Task<Product?> GetProductNameInProduct(string productName, Guid userId);
    Task<Archive?> GetProductNameInArchive(string productName, Guid userId);
    Task SaveProduct(Product product);
    Task UpdateProduct(ProductDTOs.UpdateProductDTOs updateProductDTOs, Product product);
    Task SaveProductInArchive(Guid userId, Product product);
    Task<List<Archive>> GetAllArchiveProducts(Guid userId);
    Task RestoreProductInArchive(Guid userId, Archive archive);
    Task DeleteProductInArchive(Archive archive);
}
