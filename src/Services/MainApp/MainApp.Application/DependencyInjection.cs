using System.Reflection;
using BuildingBlocks.Messaging.Masstransit;
using FluentValidation;
using MainApp.Application.Behaviors;
using MainApp.Application.Common.Options;
using MainApp.Application.Common.Services.Email;
using MainApp.Domain.Entity;
using MainApp.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MainApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>()
            .AddDefaultTokenProviders();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.Key));
        services.AddScoped<IEmailService, EmailService>();

        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

        return services;
    }
}
