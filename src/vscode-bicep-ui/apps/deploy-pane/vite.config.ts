// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import react from "@vitejs/plugin-react";
import { coverageConfigDefaults, defineConfig } from "vitest/config";
import { ViteEjsPlugin } from "vite-plugin-ejs";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), ViteEjsPlugin()],
  build: {
    rollupOptions: {
      output: {
        entryFileNames: `[name].js`,
        chunkFileNames: `chunks/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
  test: {
    globals: true,
    watch: false,
    restoreMocks: true,
    environment: 'jsdom',
    coverage: {
      enabled: true,
      exclude: ["src/index.tsx", ...coverageConfigDefaults.exclude],
    },
    setupFiles: ["src/__tests__/setupGlobals.ts"],
    deps: {
      inline: ['@vscode-elements/react-elements'],
    },
    alias: {
      '@vscode-bicep-ui/components': path.resolve(__dirname, '../../packages/components/src/index.ts')
    }
  }
});
