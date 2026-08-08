from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import Literal
import datetime

app = FastAPI(title="SS App Launcher - Python API", version="1.0.0")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)


class AppInfo(BaseModel):
    id: str
    name: str
    description: str
    url: str
    icon: str
    status: Literal["online", "offline", "loading"]


APPS: list[AppInfo] = [
    AppInfo(id="1", name="Python API", description="FastAPI backend service", url="/api/python/docs", icon="🐍", status="online"),
    AppInfo(id="2", name=".NET API", description="ASP.NET Core backend service", url="/api/dotnet/swagger", icon="⚙️", status="online"),
    AppInfo(id="3", name="Dashboard", description="Analytics and monitoring", url="#", icon="📊", status="loading"),
    AppInfo(id="4", name="Admin Panel", description="System administration", url="#", icon="🛠️", status="offline"),
]


@app.get("/health")
def health():
    return {"status": "healthy", "timestamp": datetime.datetime.utcnow().isoformat()}


@app.get("/apps", response_model=list[AppInfo])
def list_apps():
    return APPS


@app.get("/apps/{app_id}", response_model=AppInfo)
def get_app(app_id: str):
    for a in APPS:
        if a.id == app_id:
            return a
    from fastapi import HTTPException
    raise HTTPException(status_code=404, detail="App not found")
