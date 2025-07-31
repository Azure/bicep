// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import react from "@vitejs/plugin-react";
import { ViteEjsPlugin } from "vite-plugin-ejs";
import { coverageConfigDefaults, defineConfig } from "vitest/config";

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
    // A workaround for "TypeError: Cannot add property element, object is not extensible".
    // Some @vscode-elements/react-elements components extend ValidityState of the attached internals
    // with custom properties, which is not good practice, but is supported by most browsers.
    // However, element-internals-polyfill freezes the ValidityState object, causing this error.
    // There is an issue about this: https://github.com/calebdwilliams/element-internals-polyfill/issues/141.
    dangerouslyIgnoreUnhandledErrors: true,
    globals: true,
    watch: false,
    restoreMocks: true,
    environment: "happy-dom",
    coverage: {
      enabled: true,
      exclude: ["src/index.tsx", ...coverageConfigDefaults.exclude],
    },
    setupFiles: ["src/__tests__/setupGlobals.ts"],
    server: {
      deps: {
        inline: ["@vscode-elements/react-elements"],
      },
    },
    deps: {
      optimizer: {
        web: {
          include: ["@vscode-elements/react-elements"],
        },
      },
    },
  },
});
