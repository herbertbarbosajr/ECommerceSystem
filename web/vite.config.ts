import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tsconfigPaths from 'vite-tsconfig-paths'
import * as path from 'path'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), tsconfigPaths()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    host: true,
    port: 3000,
    // Proxy removido - no Docker usamos VITE_API_BASE_URL diretamente
    // Para desenvolvimento local, você pode adicionar o proxy de volta se necessário
    watch: {
      usePolling: true
    }
  }
})
