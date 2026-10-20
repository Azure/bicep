// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Plugin } from "vite";

import fs from "fs";
import path from "path";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vitest/config";

/**
 * Inject a fake `acquireVsCodeApi` with sample graph data only
 * during `vite preview`.  This lets us verify the production build
 * locally without polluting the actual bundle.
 */
function previewMock(): Plugin {
  const mockScript = `
<script>
  var graph = {
    nodes: [
      { id: "vnet", kind: "resource", parentId: null, type: "Microsoft.Network/virtualNetworks", symbolName: "vnet", isCollection: false, hasChildren: false, hasError: false },
      { id: "subnet", kind: "resource", parentId: null, type: "Microsoft.Network/virtualNetworks/subnets", symbolName: "subnet", isCollection: false, hasChildren: false, hasError: false },
      { id: "nsg", kind: "resource", parentId: null, type: "Microsoft.Network/networkSecurityGroups", symbolName: "nsg", isCollection: false, hasChildren: false, hasError: false },
      { id: "pip", kind: "resource", parentId: null, type: "Microsoft.Network/publicIPAddresses", symbolName: "pip", isCollection: true, hasChildren: false, hasError: false },
    ],
    edges: [
      { id: "subnet->vnet", sourceId: "subnet", targetId: "vnet" },
      { id: "nsg->subnet", sourceId: "nsg", targetId: "subnet" },
      { id: "pip->nsg", sourceId: "pip", targetId: "nsg" },
    ],
  };

  var positions = [
    { nodeId: "vnet", x: 0, y: 0 },
    { nodeId: "subnet", x: 190, y: 90 },
    { nodeId: "nsg", x: 380, y: 180 },
    { nodeId: "pip", x: 570, y: 270 },
  ];

  window.acquireVsCodeApi = function () {
    return {
      postMessage: function (msg) {
        console.log("[fake vscode-api] postMessage:", msg);
        if (msg && msg.method === "webview/ready") {
          setTimeout(function () {
            window.postMessage({
              method: "document/didChange",
              params: { documentUri: "file:///main.bicep" },
            }, "*");
          }, 100);
        } else if (msg && msg.method === "graph/get") {
          window.postMessage({
            id: msg.id,
            result: { graph: graph, targetScope: "resourceGroup", errorCount: 0, replayableSourceSteps: [] },
          }, "*");
        } else if (msg && msg.method === "graph/layout") {
          window.postMessage({
            id: msg.id,
            result: { status: "ok", positions: positions, bounds: { width: 760, height: 420 } },
          }, "*");
        }
      },
      getState: function () { return undefined; },
      setState: function () {},
    };
  };
</script>`;

  return {
    name: "preview-mock",
    configurePreviewServer(server) {
      const distDir = path.resolve(__dirname, "dist");
      server.middlewares.use((req, res, next) => {
        if (req.url === "/" || req.url === "/index.html") {
          const html = fs.readFileSync(path.join(distDir, "index.html"), "utf-8");
          const injected = html.replace("</head>", `${mockScript}\n</head>`);
          res.setHeader("Content-Type", "text/html");
          res.end(injected);
          return;
        }
        next();
      });
    },
  };
}

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), previewMock()],
  server: {
    watch: {
      // Playwright trace snapshots are HTML, but are not application entry points to hot-reload.
      ignored: ["**/e2e/.results/**", "**/e2e/.report/**"],
    },
  },
  resolve: {
    alias: [
      {
        find: "@/",
        replacement: path.resolve(__dirname, "src") + "/",
      },
      {
        find: "@node_modules",
        replacement: path.resolve(__dirname, "../../node_modules"),
      },
    ],
  },
  build: {
    rolldownOptions: {
      output: {
        entryFileNames: `[name].js`,
        chunkFileNames: `chunks/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
        codeSplitting: {
          groups: [
            {
              name: "azure-icons",
              test: /icon-service-.*\.js$/,
            },
          ],
        },
      },
    },
  },
  test: {
    // Unit tests live next to the source under `src`. Playwright e2e specs in `e2e/`
    // are run by Playwright, not Vitest, so keep them out of test discovery.
    include: ["src/**/*.test.{ts,tsx}"],
  },
});
