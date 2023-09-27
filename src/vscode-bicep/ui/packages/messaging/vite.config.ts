/// <reference types="vitest" />
import { defineConfig } from "vite";
import dts from "vite-plugin-dts";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    dts({
      exclude: ["**/*.test.ts"],
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
    environment: "happy-dom",
    setupFiles: ["./src/__mocks__/global.ts"],
  },
});
