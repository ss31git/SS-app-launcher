import azure.functions as func
import json
import datetime

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

APPS = [
    {"id": "1", "name": "Python API",   "description": "Azure Functions Python service",  "url": "/api/python/health", "icon": "🐍", "status": "online"},
    {"id": "2", "name": ".NET API",     "description": "Azure Functions C# service",       "url": "/api/dotnet/health", "icon": "⚙️", "status": "online"},
    {"id": "3", "name": "Dashboard",    "description": "Analytics and monitoring",          "url": "#",                  "icon": "📊", "status": "loading"},
    {"id": "4", "name": "Admin Panel",  "description": "System administration",             "url": "#",                  "icon": "🛠️", "status": "offline"},
]


@app.route(route="health")
def health(req: func.HttpRequest) -> func.HttpResponse:
    body = json.dumps({"status": "healthy", "timestamp": datetime.datetime.utcnow().isoformat()})
    return func.HttpResponse(body, mimetype="application/json")


@app.route(route="apps")
def list_apps(req: func.HttpRequest) -> func.HttpResponse:
    return func.HttpResponse(json.dumps(APPS), mimetype="application/json")


@app.route(route="apps/{app_id}")
def get_app(req: func.HttpRequest, app_id: str) -> func.HttpResponse:
    match = next((a for a in APPS if a["id"] == app_id), None)
    if match is None:
        return func.HttpResponse(json.dumps({"error": "Not found"}), status_code=404, mimetype="application/json")
    return func.HttpResponse(json.dumps(match), mimetype="application/json")
