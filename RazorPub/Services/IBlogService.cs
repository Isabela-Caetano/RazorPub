using RazorPub.Models;

namespace RazorPub.Services;

public interface IBlogService
{
    Task<IReadOnlyList<BlogPost>> GetPublishedAsync();

    Task<IReadOnlyList<BlogPost>> GetAllAsync();

    Task<BlogPost?> GetByIdAsync(int id);

    Task CreateAsync(BlogPost post);

    Task UpdateAsync(BlogPost post);

    Task DeleteAsync(int id);

    Task TogglePublishedAsync(int id);
}