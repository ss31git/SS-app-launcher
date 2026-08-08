using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ApiDotnet;

public record AppInfo(string Id, string Name, string Description, string Url, string Icon, string Status);

public class AppsFunction
{
    private static readonly List<AppInfo> Apps =
    [
        new("1", "Python API",  "Azure Functions Python service", "/api/python/health", "🐍", "online"),
        new("2", ".NET API",    "Azure Functions C# service",     "/api/dotnet/health", "⚙️", "online"),
        new("3", "Dashboard",   "Analytics and monitoring",       "#",                  "📊", "loading"),
        new("4", "Admin Panel", "System administration",          "#",                  "🛠️", "offline"),
    ];

    [Function("GetApps")]
    public IActionResult GetAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apps")] HttpRequest req)
        => new OkObjectResult(Apps);

    [Function("GetAppById")]
    public IActionResult GetById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "apps/{id}")] HttpRequest req,
        string id)
    {
        var app = Apps.FirstOrDefault(a => a.Id == id);
        return app is null ? new NotFoundResult() : new OkObjectResult(app);
    }

    [Function("Health")]
    public IActionResult Health(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
        => new OkObjectResult(new { status = "healthy", timestamp = DateTime.UtcNow });
}
