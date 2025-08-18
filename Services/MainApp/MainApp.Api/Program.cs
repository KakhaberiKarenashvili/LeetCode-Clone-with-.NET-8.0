using MainApp.Api;
using MainApp.Api.Middlewares;
using MainApp.Application;
using MainApp.Domain.Entity;
using MainApp.Infrastructure;
using MainApp.Infrastructure.Data.Seeder;
using MainApp.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddPresentationServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);
    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDatabase();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>(); 

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeeder.SeedRoles(roleManager);
        logger.LogInformation("Roles Seeded Successfully");
        
        var userManager = services.GetRequiredService<UserManager<User>>();
        await AdminSeeder.SeedAdmin(userManager);
        logger.LogInformation("Admin User Seeded Successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding roles or admin user.");
    }
}

app.Run();