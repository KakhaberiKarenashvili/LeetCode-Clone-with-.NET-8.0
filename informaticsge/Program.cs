using System.Text;
using informaticsge.Data.Seeder;
using informaticsge.Entity;
using informaticsge.Filters;
using informaticsge.JWT;
using informaticsge.Middlewares;
using informaticsge.Models;
using informaticsge.Modules;
using informaticsge.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341/")
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new PlainTextInputFormatter());
        options.Filters.Add<ModelValidationActionFilter>();
    })
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dbcontext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DBConnection")));

//to be able to inject JWTService into our controler
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProblemsService>();
builder.Services.AddHttpClient();

//adding identity service as user manager and signin manager
builder.Services.AddIdentity<User,IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddUserManager<UserManager<User>>()
    .AddDefaultTokenProviders();

//to be able authenticate user by JWT 
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

//adding authentication before authorization
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
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the roles.");
    }
}

app.Run();