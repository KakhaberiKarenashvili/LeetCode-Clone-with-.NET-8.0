using MainApp.Domain.Entity;
using Microsoft.AspNetCore.Identity;

namespace MainApp.Infrastructure.Data.Seeder;

public static class AdminSeeder
{
    public static async Task SeedAdmin(UserManager<User> userManager)
    {
            var admin = await userManager.FindByNameAsync("Admin");
        
            if (admin == null)
            {
                admin = new User
                {
                    UserName = "Admin",
                    Email = "Admin@example.com",
                };
                
                var result = await userManager.CreateAsync(admin, "Admin-1231");
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
    }
}