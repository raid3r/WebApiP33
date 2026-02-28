import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    server: {
      port: 52356,
      proxy: {
        '/api': {
          target: 'https://localhost:7025',
          changeOrigin: true,
          secure: false,
          //rewrite: (path) => path.replace(/^\/api/, '/api')
        }
      }
    }
})
