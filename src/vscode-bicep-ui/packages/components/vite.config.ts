// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import react from "@vitejs/plugin-react";
import dts from "vite-plugin-dts";
import { libInjectCss } from "vite-plugin-lib-inject-css";
import svgr from "vite-plugin-svgr";
import { coverageConfigDefaults, defineConfig } from "vitest/config";
import packageJson from "./package.json" with { type: "json" };

const peerDependencies = Object.keys(packageJson.peerDependencies);

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    react(),
    svgr(),
    dts({
      include: ["src/**/*.ts", "src/**/*.tsx"],
      exclude: ["src/**/*.stories.ts", "src/**/*.stories.tsx", "src/**/__tests__/**"],
      insertTypesEntry: true,
    }),
    libInjectCss(),
  ],
  build: {
    lib: {
      formats: ["es"],
      entry: {
        index: "src/index.ts",
        accordion: "src/accordion/index.ts",
        "azure-icon": "src/azure-icon/index.ts",
        codicon: "src/codicon/index.ts",
        list: "src/list/index.ts",
        "pan-zoom": "src/pan-zoom/index.ts",
      },
    },
    rolldownOptions: {
      external: (id) => peerDependencies.some((dependency) => id === dependency || id.startsWith(`${dependency}/`)),
      output: {
        entryFileNames: "[name].js",
        chunkFileNames: "chunks/[name].[hash].js",
        assetFileNames: "assets/[name][extname]",
      },
    },
  },
  test: {
    testTimeout: 15000,
    watch: false,
    globals: true,
    restoreMocks: true,
    environment: "happy-dom",
    coverage: {
      enabled: true,
      exclude: ["src/**/*.stories.ts", "src/**/*.stories.tsx", "src/**/index.ts", ...coverageConfigDefaults.exclude],
    },
    setupFiles: ["src/__tests__/extendMatchers.ts"],
  },
});
