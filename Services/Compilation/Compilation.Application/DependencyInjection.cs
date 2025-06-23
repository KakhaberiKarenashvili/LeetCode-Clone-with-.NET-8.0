using System.Reflection;
using BuildingBlocks.Messaging.MassTransit;
using Compilation.Application.Services;

namespace Compilation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CppTestingService>();
        services.AddScoped<PythonTestingService>();
        services.AddTransient<MemoryMonitorService>();
        
        services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());
        
        return services;
    }
}