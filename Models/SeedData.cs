using Microsoft.AspNetCore.Identity;

public class SeedData
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Administrator", "User", "Finance", "Inventory", "Content"];

        foreach (string role in roles)
        {
            if (await roleManager.FindByNameAsync(role) is null)
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}