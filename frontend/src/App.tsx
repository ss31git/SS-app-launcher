import { useState, useEffect } from 'react'
import './App.css'
import PrimalityTester from './PrimalityTester'

interface App {
  id: string
  name: string
  description: string
  url: string
  icon: string
  status: 'online' | 'offline' | 'loading'
}

const DEMO_APPS: App[] = [
  { id: '1', name: 'Python API', description: 'FastAPI backend service', url: '/api/python/docs', icon: '🐍', status: 'online' },
  { id: '3', name: 'Dashboard', description: 'Analytics and monitoring', url: '#', icon: '📊', status: 'loading' },
  { id: '4', name: 'Admin Panel', description: 'System administration', url: '#', icon: '🛠️', status: 'offline' },
  { id: '5', name: 'Primality Tester', description: 'Check whether a number is prime', url: '', icon: '🔢', status: 'online' },
]

export default function App() {
  const [apps] = useState<App[]>(DEMO_APPS)
  const [search, setSearch] = useState('')
  const [filter, setFilter] = useState<'all' | 'online' | 'offline'>('all')
  const [view, setView] = useState<'grid' | 'primality'>('grid')

  const filtered = apps.filter(app => {
    const matchSearch = app.name.toLowerCase().includes(search.toLowerCase())
    const matchFilter = filter === 'all' || app.status === filter
    return matchSearch && matchFilter
  })

  if (view === 'primality') {
    return (
      <div className="layout">
        <header className="header">
          <div className="header-inner">
            <h1 className="logo">SS App Launcher</h1>
          </div>
        </header>
        <PrimalityTester onBack={() => setView('grid')} />
      </div>
    )
  }

  return (
    <div className="layout">
      <header className="header">
        <div className="header-inner">
          <h1 className="logo">SS App Launcher</h1>
          <span className="badge">{apps.filter(a => a.status === 'online').length} online</span>
        </div>
      </header>

      <main className="main">
        <div className="controls">
          <input
            className="search"
            type="text"
            placeholder="Search apps..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
          <div className="filters">
            {(['all', 'online', 'offline'] as const).map(f => (
              <button key={f} className={`filter-btn ${filter === f ? 'active' : ''}`} onClick={() => setFilter(f)}>
                {f.charAt(0).toUpperCase() + f.slice(1)}
              </button>
            ))}
          </div>
        </div>

        <div className="grid">
          {filtered.map(app =>
            app.id === '5' ? (
              <button
                key={app.id}
                className={`card status-${app.status}`}
                onClick={() => setView('primality')}
                style={{ font: 'inherit', textAlign: 'left', cursor: 'pointer' }}
              >
                <div className="card-icon">{app.icon}</div>
                <div className="card-body">
                  <h2 className="card-name">{app.name}</h2>
                  <p className="card-desc">{app.description}</p>
                </div>
                <span className={`status-dot dot-${app.status}`} />
              </button>
            ) : (
              <a key={app.id} href={app.url} className={`card status-${app.status}`} target="_blank" rel="noreferrer">
                <div className="card-icon">{app.icon}</div>
                <div className="card-body">
                  <h2 className="card-name">{app.name}</h2>
                  <p className="card-desc">{app.description}</p>
                </div>
                <span className={`status-dot dot-${app.status}`} />
              </a>
            )
          )}
        </div>

        {filtered.length === 0 && (
          <p className="empty">No apps found matching "{search}"</p>
        )}
      </main>
    </div>
  )
}
