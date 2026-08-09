import { useState } from 'react'

interface Result {
  number: string
  isPrime: boolean
  method: string
  smallestFactor: string | null
}

interface ApiError {
  error: string
}

export default function PrimalityTester({ onBack }: { onBack: () => void }) {
  const [value, setValue] = useState('')
  const [result, setResult] = useState<Result | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const check = async () => {
    setResult(null)
    setError(null)
    setLoading(true)
    try {
      const apiBase = import.meta.env.VITE_API_DOTNET_BASE ?? '/api/dotnet'
      const res = await fetch(`${apiBase}/primality/${encodeURIComponent(value)}`)
      const data = await res.json()
      if (!res.ok) {
        setError((data as ApiError).error ?? 'Something went wrong.')
      } else {
        setResult(data as Result)
      }
    } catch {
      setError('Could not reach the API.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="main">
      <button className="filter-btn" onClick={onBack} style={{ marginBottom: '1.5rem' }}>
        ← Back
      </button>

      <h2 className="card-name" style={{ fontSize: '1.3rem', marginBottom: '1rem' }}>
        Primality Tester
      </h2>

      <div className="controls">
        <input
          className="search"
          type="text"
          inputMode="numeric"
          placeholder="Enter a whole number (any size, up to 2000 digits)"
          value={value}
          onChange={e => setValue(e.target.value)}
          onKeyDown={e => e.key === 'Enter' && check()}
        />
        <button className="filter-btn active" onClick={check} disabled={loading || value.trim() === ''}>
          {loading ? 'Checking...' : 'Check'}
        </button>
      </div>

      {error && <p className="empty">{error}</p>}

      {result && (
        <div className="card" style={{ maxWidth: 420 }}>
          <div className="card-icon">{result.isPrime ? '✅' : '❌'}</div>
          <div className="card-body">
            <h2 className="card-name">
              {result.number} is {result.isPrime ? 'prime' : 'not prime'}
            </h2>
            {!result.isPrime && result.smallestFactor && (
              <p className="card-desc">Smallest factor: {result.smallestFactor}</p>
            )}
            <p className="card-desc">Method: {result.method}</p>
          </div>
        </div>
      )}
    </div>
  )
}
