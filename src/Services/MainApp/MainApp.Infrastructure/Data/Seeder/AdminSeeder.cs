using MainApp.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MainApp.Infrastructure.Data.Seeder;

public static class AdminSeeder
{
    public static async Task SeedAdmin(UserManager<User> userManager, IConfiguration config)
    {
            var admin = await userManager.FindByNameAsync("Admin");
        
            if (admin == null)
            {
                admin = new User
                {
                    UserName = config.GetSection("AdminUser:UserName").Get<string>(),
                    Email = config.GetSection("AdminUser:Email").Get<string>(),
                };
                
                var result = await userManager.CreateAsync(admin, config.GetSection("AdminUser:Password").Get<string>());
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
    }
}