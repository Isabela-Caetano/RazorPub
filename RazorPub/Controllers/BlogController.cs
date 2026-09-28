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
        IFormFile? coverImage)
    {
        if (!ModelState.IsValid)
        {
            return View(post);
        }

        try
        {
            post.CoverImagePath =
                await imageStorage.SaveAsync(coverImage);

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
       bool removeCoverImage)
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
                post.CoverImagePath = await imageStorage.SaveAsync(coverImage);
            }
            else if (removeCoverImage)
            {
                post.CoverImagePath = null;
            }

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
}