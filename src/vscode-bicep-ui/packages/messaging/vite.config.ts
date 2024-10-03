// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import dts from "vite-plugin-dts";
import { coverageConfigDefaults, defineConfig } from "vitest/config";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    dts({
      exclude: ["**/__tests__/**"],
    }),
  ],
  build: {
    lib: {
      formats: ["es"],
      entry: "src/index.ts",
    },
    rollupOptions: {
      output: {
        entryFileNames: "[name].js",
        chunkFileNames: "chunks/[name].[hash].js",
        assetFileNames: "assets/[name][extname]",
      },
    },
  },
  test: {
    watch: false,
    globals: true,
    restoreMocks: true,
    environment: "happy-dom",
    coverage: {
      enabled: true,
      exclude: ["src/index.ts", ...coverageConfigDefaults.exclude],
    },
    setupFiles: ["src/__tests__/setupGlobals.ts"],
    onConsoleLog: (log) => {
      return !log.includes("Consider adding an error boundary to your tree to customize error handling behavior.");
    },
  },
});
