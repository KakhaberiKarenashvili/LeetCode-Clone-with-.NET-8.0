using Compilation_Service.Dto.Response;
using Compilation_Service.RabbitMQ;
using Compilation_Service.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.Seq("http://host.docker.internal:5341/")
    .CreateLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register services
builder.Services.AddScoped<MemoryMonitorService>();
builder.Services.AddSingleton<RabbitMqService>(); 
builder.Services.AddSingleton<RequestListener>();
builder.Services.AddScoped<CppTestingService>();
builder.Services.AddScoped<PythonTestingService>();
builder.Services.AddScoped<SubmissionRequestHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var rabbitMqService = services.GetRequiredService<RabbitMqService>();
    var requestListener = new RequestListener(rabbitMqService, services.GetRequiredService<IServiceScopeFactory>(), services.GetRequiredService<ILogger<RequestListener>>());
}


app.Run();
