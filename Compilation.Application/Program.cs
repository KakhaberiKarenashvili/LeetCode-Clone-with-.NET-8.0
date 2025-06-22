using Compilation.Application;
using Compilation.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var host = builder.Build();

host.Run();