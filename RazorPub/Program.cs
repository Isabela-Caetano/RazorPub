using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RazorPub.Data;
using RazorPub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RegistrationDisabled", policy =>
        policy.RequireAssertion(_ => false));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaPage(
        "Identity",
        "/Account/Register",
        "RegistrationDisabled");
});

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "blogPost",
    pattern: "blog/{slug}",
    defaults: new
    {
        controller = "Blog",
        action = "DetailsBySlug"
    });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

await SeedData.InitializeAsync(app.Services);

app.Run();