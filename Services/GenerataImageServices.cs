using System;
using System.IO;
using System.Threading.Tasks;
using Server.DTOs;

namespace Server.Services;

public class GenerateImageServices
{
    public async Task<string?> Generate(ProductDTOs.CreateProductDTOs createProductDto)
    {
        if (createProductDto.Image is null)
            return null;

        if (!createProductDto.Image.ContentType.StartsWith("image/"))
            throw new ArgumentException("Invalid image format");

        if (createProductDto.Image.Length > 5 * 1024 * 1024)
            throw new ArgumentException("Image size cannot exceed 5MB");

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "products"
        );

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(createProductDto.Image.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await createProductDto.Image.CopyToAsync(stream);

        return $"/products/{fileName}";
    }
}
