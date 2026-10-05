using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RazorPub.Models;
using RazorPub.Services;

namespace RazorPub.Controllers;

public class BlogController(
    IBlogService blogService,
    IImageStorageService imageStorage) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await blogService.GetPublishedAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var post = await blogService.GetByIdAsync(id);

        return post is null || !post.IsPublished
            ? NotFound()
            : View(post);
    }

    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        var post = await blogService.GetBySlugAsync(slug);

        return post is null || !post.IsPublished
            ? NotFound()
            : View("Details", post);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        return View(await blogService.GetAllAsync());
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View(new BlogPost());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        BlogPost post,
        IFormFile? coverImage,
        IFormFile? contentImage)
    {
        if (!ModelState.IsValid)
        {
            return View(post);
        }

        try
        {
            post.CoverImagePath =
                await imageStorage.SaveAsync(coverImage);

            post.ContentImagePath =
                await imageStorage.SaveAsync(contentImage);

            if (string.IsNullOrWhiteSpace(post.CoverImagePath))
            {
                post.CoverImagePath = post.ContentImagePath;
            }

            if (string.IsNullOrWhiteSpace(post.ContentImagePath))
            {
                post.ContentImagePath = post.CoverImagePath;
            }

            post.Slug = await GenerateUniqueSlugAsync(post.Title);

            await blogService.CreateAsync(post);

            return RedirectToAction(nameof(Manage));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError("coverImage", exception.Message);

            return View(post);
        }
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await blogService.GetByIdAsync(id);

        return post is null ? NotFound() : View(post);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(
        int id,
        BlogPost post,
        IFormFile? coverImage,
        IFormFile? contentImage,
        bool removeCoverImage,
        bool removeContentImage)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(post);
        }

        try
        {
            if (coverImage is not null && coverImage.Length > 0)
            {
                post.CoverImagePath =
                    await imageStorage.SaveAsync(coverImage);
            }
            else if (removeCoverImage)
            {
                post.CoverImagePath = null;
            }

            if (contentImage is not null && contentImage.Length > 0)
            {
                post.ContentImagePath =
                    await imageStorage.SaveAsync(contentImage);
            }
            else if (removeContentImage)
            {
                post.ContentImagePath = null;
            }

            if (string.IsNullOrWhiteSpace(post.CoverImagePath))
            {
                post.CoverImagePath = post.ContentImagePath;
            }

            if (string.IsNullOrWhiteSpace(post.ContentImagePath))
            {
                post.ContentImagePath = post.CoverImagePath;
            }

            post.Slug = await GenerateUniqueSlugAsync(post.Title, post.Id);

            await blogService.UpdateAsync(post);

            return RedirectToAction(nameof(Manage));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError("coverImage", exception.Message);

            return View(post);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await blogService.DeleteAsync(id);

        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TogglePublished(int id)
    {
        await blogService.TogglePublishedAsync(id);

        return RedirectToAction(nameof(Manage));
    }

    private async Task<string> GenerateUniqueSlugAsync(
        string title,
        int? currentPostId = null)
    {
        var baseSlug = GenerateSlug(title);

        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "post";
        }

        var slug = baseSlug;
        var suffix = 2;

        while (true)
        {
            var existingPost = await blogService.GetBySlugAsync(slug);

            if (existingPost is null || existingPost.Id == currentPostId)
            {
                return slug;
            }

            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }
    }

    private static string GenerateSlug(string text)
    {
        var normalizedText = text.Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder();
        var previousCharacterWasDash = false;

        foreach (var character in normalizedText)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                slug.Append(char.ToLowerInvariant(character));
                previousCharacterWasDash = false;
            }
            else if (slug.Length > 0 && !previousCharacterWasDash)
            {
                slug.Append('-');
                previousCharacterWasDash = true;
            }
        }

        return slug.ToString().Trim('-');
    }
}