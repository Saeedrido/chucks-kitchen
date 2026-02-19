using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ChuksKitchen.API.Controllers;

/// <summary>
/// Health check and system information endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public HealthController(
        ILogger<HealthController> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Simple health status</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            message = "Chuks Kitchen API is running"
        });
    }

    /// <summary>
    /// Detailed health check with service status
    /// </summary>
    /// <returns>Detailed health status including database and services</returns>
    [HttpGet("detailed")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDetailed()
    {
        var startTime = DateTime.UtcNow;
        var dbHealth = await CheckDatabaseHealth();

        var healthStatus = new
        {
            status = "healthy",
            timestamp = startTime,
            api = new
            {
                name = "Chuks Kitchen API",
                version = "v1.0.0",
                environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Unknown",
                uptime = GetUptime()
            },
            database = dbHealth,
            services = new
            {
                emailService = CheckService("EmailService"),
                smsService = CheckService("SmsService"),
                otpService = CheckService("OtpService")
            },
            responseTime = $"{(DateTime.UtcNow - startTime).TotalMilliseconds:F2}ms"
        };

        return Ok(healthStatus);
    }

    /// <summary>
    /// Readiness probe - checks if API is ready to accept requests
    /// </summary>
    [HttpGet("ready")]
    [AllowAnonymous]
    public async Task<IActionResult> Ready()
    {
        var dbHealthy = await CheckDatabaseHealth();

        if (dbHealthy is Dictionary<string, object> dict && dict.ContainsKey("status"))
        {
            var status = dict["status"]?.ToString();
            if (status != "healthy")
            {
                return StatusCode(503, new
                {
                    status = "not ready",
                    timestamp = DateTime.UtcNow,
                    reason = "Database is not ready"
                });
            }
        }

        return Ok(new
        {
            status = "ready",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Liveness probe - checks if API is alive
    /// </summary>
    [HttpGet("live")]
    [AllowAnonymous]
    public IActionResult Live()
    {
        return Ok(new
        {
            status = "alive",
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get system information
    /// </summary>
    [HttpGet("system")]
    [AllowAnonymous]
    public IActionResult GetSystemInfo()
    {
        var systemInfo = new
        {
            timestamp = DateTime.UtcNow,
            server = new
            {
                time = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                timeZone = "UTC",
                machineName = Environment.MachineName,
                osVersion = Environment.OSVersion.ToString(),
                processorCount = Environment.ProcessorCount,
                workingSet = $"{Environment.WorkingSet / 1024 / 1024}MB"
            },
            api = new
            {
                name = "Chuks Kitchen Food Ordering API",
                version = "1.0.0",
                environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Unknown",
                framework = ".NET 8.0",
                uptime = GetUptime()
            },
            endpoints = new
            {
                baseUrl = Request.Scheme + "://" + Request.Host,
                swagger = $"{Request.Scheme}://{Request.Host}/swagger",
                health = $"{Request.Scheme}://{Request.Host}/health",
                apiBase = $"{Request.Scheme}://{Request.Host}/api/v1"
            }
        };

        return Ok(systemInfo);
    }

    /// <summary>
    /// Get server time
    /// </summary>
    [HttpGet("time")]
    [AllowAnonymous]
    public IActionResult GetServerTime()
    {
        return Ok(new
        {
            timestamp = DateTime.UtcNow,
            iso8601 = DateTime.UtcNow.ToString("o"),
            readable = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            timeZone = "UTC"
        });
    }

    private async Task<object> CheckDatabaseHealth()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<ChuksKitchen.Persistence.Data.AppDbContext>();

            if (context == null)
            {
                return new { status = "unhealthy", message = "Database context not available" };
            }

            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                return new { status = "unhealthy", message = "Cannot connect to database" };
            }

            return new
            {
                status = "healthy",
                type = "InMemory",
                message = "Database connection successful"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return new { status = "unhealthy", message = ex.Message };
        }
    }

    private object CheckService(string serviceName)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var serviceType = System.Type.GetType($"ChuksKitchen.Infrastructure.Services.I{serviceName}, ChuksKitchen.Infrastructure");

            if (serviceType != null)
            {
                var service = scope.ServiceProvider.GetService(serviceType);
                return new { status = "registered", service = serviceName };
            }

            return new { status = "available", service = serviceName };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Service check failed for {ServiceName}", serviceName);
            return new { status = "error", service = serviceName, message = ex.Message };
        }
    }

    private string GetUptime()
    {
        var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

        if (uptime.TotalDays >= 1)
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m";
        else if (uptime.TotalHours >= 1)
            return $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        else
            return $"{uptime.Minutes}m {uptime.Seconds}s";
    }
}
