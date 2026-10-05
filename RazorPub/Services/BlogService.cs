using Microsoft.EntityFrameworkCore;
using RazorPub.Data;
using RazorPub.Models;

namespace RazorPub.Services;

public class BlogService(ApplicationDbContext context) : IBlogService
{
    public async Task<IReadOnlyList<BlogPost>> GetPublishedAsync()
    {
        return await context.BlogPosts
            .Where(post => post.IsPublished)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BlogPost>> GetAllAsync()
    {
        return await context.BlogPosts
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogPost?> GetByIdAsync(int id)
    {
        return await context.BlogPosts.FindAsync(id);
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug)
    {
        return await context.BlogPosts
            .FirstOrDefaultAsync(post => post.Slug == slug);
    }

    public async Task CreateAsync(BlogPost post)
    {
        post.CreatedAt = DateTime.UtcNow;

        context.BlogPosts.Add(post);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BlogPost incomingPost)
    {
        var post = await GetByIdAsync(incomingPost.Id);

        if (post is null)
        {
            return;
        }

        post.Title = incomingPost.Title;
        post.Summary = incomingPost.Summary;
        post.Content = incomingPost.Content;
        post.IsPublished = incomingPost.IsPublished;
        post.CoverImagePath = incomingPost.CoverImagePath;
        post.ContentImagePath = incomingPost.ContentImagePath;
        post.Slug = incomingPost.Slug;

        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var post = await GetByIdAsync(id);

        if (post is null)
        {
            return;
        }

        context.BlogPosts.Remove(post);
        await context.SaveChangesAsync();
    }

    public async Task TogglePublishedAsync(int id)
    {
        var post = await GetByIdAsync(id);

        if (post is null)
        {
            return;
        }

        post.IsPublished = !post.IsPublished;
        await context.SaveChangesAsync();
    }
}