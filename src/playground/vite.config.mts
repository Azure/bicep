import { defineConfig, normalizePath } from 'vite'
import { viteStaticCopy } from 'vite-plugin-static-copy'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    viteStaticCopy({
      targets: [
        {
          src: normalizePath('../Bicep.Wasm/bin/Release/net10.0/wwwroot/_framework/**/*.*'),
          dest: './_framework',
          rename: { stripBase: 6 },
        },
      ],
    })
  ],
  base: "./"
})
