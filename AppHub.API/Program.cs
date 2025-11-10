using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using AppHub.Infrastructure.Data;
using AppHub.Infrastructure.UnitOfWork;
using AppHub.Application.Services;
using AppHub.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AppHub API",
        Description = "AppHub API Documentation",
        Contact = new OpenApiContact
        {
            Name = "AppHub Team"
        }
    });

    // XML comments eklemek için (opsiyonel)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppHubDbContext>(options =>
    options.UseNpgsql(connectionString));

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services
builder.Services.AddScoped<IOAuthProviderService, OAuthProviderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserOAuthService, UserOAuthService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "AppHub API v1");
    options.RoutePrefix = "swagger"; // Swagger UI: /swagger
    options.DisplayRequestDuration();
    options.EnableDeepLinking();
    options.EnableFilter();
    options.ShowExtensions();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
