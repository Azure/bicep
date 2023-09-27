import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import dts from "vite-plugin-dts";
import svgr from "vite-plugin-svgr";
import { libInjectCss } from "vite-plugin-lib-inject-css";
import peerDepsExternal from "rollup-plugin-peer-deps-external";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    peerDepsExternal(),
    react(),
    svgr(),
    dts({
      include: ["src/**/*.ts", "src/**/*.tsx"],
      exclude: ["src/**/*.stories.ts", "src/**/*.stories.tsx"],
      insertTypesEntry: true,
    }),
    libInjectCss(),
  ],
  build: {
    lib: {
      formats: ["es"],
      entry: {
        index: "src/index.ts",
        "azure-icon": "src/azure-icon/index.ts",
        codicon: "src/codicon/index.ts",
      },
    },
    rollupOptions: {
      output: {
        entryFileNames: "[name].js",
        chunkFileNames: "chunks/[name].[hash].js",
        assetFileNames: "assets/[name][extname]",
      },
    },
  },
});
