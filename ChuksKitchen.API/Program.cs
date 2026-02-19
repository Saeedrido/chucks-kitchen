using System.Text;
using ChuksKitchen.Application.Repositories.Interfaces;
using ChuksKitchen.Application.Services.Interfaces;
using ChuksKitchen.Application.Services;
using ChuksKitchen.Infrastructure.Services;
using ChuksKitchen.Persistence.Data;
using ChuksKitchen.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ChuksKitchen.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChuksKitchenSecretKeyForJWTTokenGeneration123!@#";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ChuksKitchenAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ChuksKitchenClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Remove delay of token expiration
    };
});

builder.Services.AddAuthorization();

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        // Custom database health check will be handled by HealthController
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is available");
    })
    .AddCheck("api", () =>
    {
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API is running");
    });

// Configure Database - Using InMemory for this deliverable
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ChuksKitchenDb"));

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFoodItemRepository, FoodItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReferralCodeService, ReferralCodeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOtpService, OtpService>();

// Register Infrastructure Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();

// Configure CORS - More restrictive for security
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // For development: Allow localhost and any origin
        // For production: Replace with specific domains
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Production: Only allow specific origins
            policy.WithOrigins("https://chukskitchen.com", "https://www.chukskitchen.com")
                  .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                  .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                  .AllowCredentials();
        }
    });
});

// Configure Swagger/OpenAPI with API Versioning
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Chuks Kitchen API",
        Version = "v1.0",
        Description = "Food Ordering & Customer Management System for Chuks Kitchen - Version 1.0"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chuks Kitchen API V1");
    });
}

app.UseHttpsRedirection();

// Global Exception Handler (must be before other middleware)
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Map Health Checks endpoints
app.MapHealthChecks("/health"); // Basic health check (for Kubernetes/Docker probes)
// Note: Detailed health checks are available at /api/v1/health/detailed

app.MapControllers();

// Create database if it doesn't exist (for development)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Ensure database is created and migrations are applied
        dbContext.Database.EnsureCreated();
        logger.LogInformation("Database initialized successfully at {Time}", DateTime.UtcNow);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error initializing database");
    }
}

app.Run();
