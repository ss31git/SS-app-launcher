using Microsoft.AspNetCore.Mvc;

namespace ApiDotnet.Controllers;

public record AppInfo(string Id, string Name, string Description, string Url, string Icon, string Status);

[ApiController]
[Route("[controller]")]
public class AppsController : ControllerBase
{
    private static readonly List<AppInfo> Apps =
    [
        new("1", "Python API",   "FastAPI backend service",        "/api/python/docs",    "🐍", "online"),
        new("2", ".NET API",     "ASP.NET Core backend service",   "/api/dotnet/swagger", "⚙️", "online"),
        new("3", "Dashboard",    "Analytics and monitoring",       "#",                   "📊", "loading"),
        new("4", "Admin Panel",  "System administration",          "#",                   "🛠️", "offline"),
    ];

    [HttpGet]
    public IActionResult GetAll() => Ok(Apps);

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var app = Apps.FirstOrDefault(a => a.Id == id);
        return app is null ? NotFound() : Ok(app);
    }

    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}
