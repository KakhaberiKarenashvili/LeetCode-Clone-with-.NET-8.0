using Microsoft.AspNetCore.Identity;

namespace informaticsge.Data.Seeder;

public static class RoleSeeder
{
    private static readonly string[] Roles = new[] { "Admin", "User" };

    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    
}