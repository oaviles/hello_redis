using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace redisapp.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiCacheController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ApiCacheController> _logger;


    public ApiCacheController(ILogger<ApiCacheController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetApiCache")]
    public IEnumerable<Cache> Get()
    {
        var rng = new Random();
        
        var connectionString = Environment.GetEnvironmentVariable("REDIS_CS");
        IDatabase db;

        try {

        if (connectionString != null)
        {
            var cache = ConnectionMultiplexer.Connect(connectionString);
            db = cache.GetDatabase();
            _logger.LogInformation("Cache is answering...");

            return Enumerable.Range(1, 5).Select(index => new Cache
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = int.Parse(db.StringGet("test:key").ToString()),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
        } 
        else
        {
            _logger.LogInformation("Default behavior is answering...");

            return Enumerable.Range(1, 5).Select(index => new Cache
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = int.Parse(rng.Next(-20, 55).ToString()),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        }

        catch (Exception ex)
        {
            _logger.LogError("Error: {0}", ex.Message);

            return Enumerable.Range(1, 5).Select(index => new Cache
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = int.Parse(rng.Next(-20, 55).ToString()),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
