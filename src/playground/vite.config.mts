import { defineConfig, normalizePath } from 'vite'
import { viteStaticCopy } from 'vite-plugin-static-copy'
import monacoEditorEsmPlugin from 'vite-plugin-monaco-editor-esm'
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    monacoEditorEsmPlugin({
      languageWorkers: ['editorWorkerService', 'json'],
    }),
    viteStaticCopy({
      targets: [
        {
          src: normalizePath(path.resolve(__dirname, '../Bicep.Wasm/bin/Release/net10.0/wwwroot/_framework') + '/**/*.*'),
          dest: './_framework',
        },
      ],
    })
  ],
  base: "./"
})