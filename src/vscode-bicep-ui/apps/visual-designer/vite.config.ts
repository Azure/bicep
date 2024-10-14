import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'


console.log(path.resolve(__dirname, "../../node_modules"));
// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: [
      {
        find: "@node_modules",
        replacement: path.resolve(__dirname, "../../node_modules")
      }
    ]
  },
  build: {
    rollupOptions: {
      output: {
        entryFileNames: `[name].js`,
        chunkFileNames: `chunks/[name].js`,
        assetFileNames: `assets/[name].[ext]`
      }
    },
  }
})