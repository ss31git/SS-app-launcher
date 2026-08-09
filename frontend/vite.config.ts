import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api/python': { target: 'http://localhost:8000', rewrite: (p) => p.replace(/^\/api\/python/, '') },
      '/api/dotnet': { target: 'http://localhost:7071', rewrite: (p) => p.replace(/^\/api\/dotnet/, '/api') },
    }
  }
})
