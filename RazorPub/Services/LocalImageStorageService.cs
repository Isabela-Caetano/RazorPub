using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace RazorPub.Services;

public class LocalImageStorageService(
    IWebHostEnvironment environment) : IImageStorageService
{
    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;

    public async Task<string?> SaveAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException(
                "A imagem deve ter no máximo 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                "Use apenas JPG, JPEG, PNG ou WebP.");
        }

        var folder = Path.Combine(
            environment.WebRootPath,
            "uploads",
            "blog");

        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid():N}{extension}";

        var physicalPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(
            physicalPath,
            FileMode.Create);

        await file.CopyToAsync(stream);

        return $"/uploads/blog/{fileName}";
    }
}
