using System.Reflection;
using BuildingBlocks.Messaging.Masstransit;
using Compilation.Application.Services;

namespace Compilation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CodeTestingService>();
        services.AddTransient<MemoryMonitorService>();
        
        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
        
        return services;
    }
}