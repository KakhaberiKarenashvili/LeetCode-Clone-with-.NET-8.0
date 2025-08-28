namespace MainApp.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options => 
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        
        return services;
    }
}