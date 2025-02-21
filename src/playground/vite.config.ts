import { defineConfig } from 'vite'
import { viteStaticCopy } from 'vite-plugin-static-copy'
import monacoEditorPlugin from 'vite-plugin-monaco-editor';
import react from '@vitejs/plugin-react'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    monacoEditorPlugin({
      languageWorkers: ['editorWorkerService', 'json'],
    }),
    viteStaticCopy({
      targets: [
        {
          src: path.resolve(__dirname, '../Bicep.Wasm/bin/Release/net8.0/wwwroot/_framework/'),
          dest: './',
        },
      ],
    })
  ],
  base: "./"
})