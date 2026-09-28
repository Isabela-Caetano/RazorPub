using Microsoft.AspNetCore.Http;

namespace RazorPub.Services;

public interface IImageStorageService
{
    Task<string?> SaveAsync(IFormFile? file);
}