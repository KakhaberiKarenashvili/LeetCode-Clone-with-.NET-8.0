using System.Reflection;
using BuildingBlocks.Messaging.MassTransit;
using MainApp.Application.Services;
using MainApp.Domain.Models;
using MainApp.Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User,IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>()
            .AddDefaultTokenProviders();
        
        services.AddScoped<AccountService>();
        services.AddScoped<AdminService>();
        services.AddScoped<UserService>();
        services.AddScoped<ProblemsService>();
        services.AddScoped<SubmissionService>();
        
        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
        
        return services;
    }
}