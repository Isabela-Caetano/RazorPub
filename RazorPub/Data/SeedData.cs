using Microsoft.AspNetCore.Identity;

namespace RazorPub.Data;

public static class SeedData

{

    public static async Task InitializeAsync(IServiceProvider services)

    {

        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider

            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider

            .GetRequiredService<UserManager<IdentityUser>>();

        var configuration = scope.ServiceProvider

            .GetRequiredService<IConfiguration>();

        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))

        {

            await roleManager.CreateAsync(new IdentityRole(adminRole));

        }

        var email = configuration["Admin:Email"];

        var password = configuration["Admin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))

        {

            return;

        }

        var admin = await userManager.FindByEmailAsync(email);

        if (admin is null)

        {

            admin = new IdentityUser

            {

                UserName = email,

                Email = email,

                EmailConfirmed = true

            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)

            {

                var errors = string.Join("; ", result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(errors);

            }

        }

        if (!await userManager.IsInRoleAsync(admin, adminRole))

        {

            await userManager.AddToRoleAsync(admin, adminRole);

        }

    }

}