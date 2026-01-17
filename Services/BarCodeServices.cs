using System;
using SkiaSharp;
using ZXing;
using ZXing.Common;

namespace Server.Services;

public class BarCodeServices
{
    public string GenerateProductBarcode(Guid productId)
    {
        var writer = new BarcodeWriterPixelData
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = 300,
                Height = 100,
                Margin = 10
            }
        };

        var pixelData = writer.Write(productId.ToString());

        using var bitmap = new SKBitmap(pixelData.Width, pixelData.Height);
        for (int y = 0; y < pixelData.Height; y++)
        {
            for (int x = 0; x < pixelData.Width; x++)
            {
                int index = (y * pixelData.Width + x) * 4;
                byte r = pixelData.Pixels[index];
                byte g = pixelData.Pixels[index + 1];
                byte b = pixelData.Pixels[index + 2];
                bitmap.SetPixel(x, y, new SKColor(r, g, b));
            }
        }

        using var image = SKImage.FromBitmap(bitmap);

        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        var bytes = data.ToArray();

        // Return Base64 string
        return Convert.ToBase64String(bytes);
    }
}
