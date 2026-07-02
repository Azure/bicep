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
      { id: "subnet>vnet", sourceId: "subnet", targetId: "vnet" },
      { id: "nsg>subnet", sourceId: "nsg", targetId: "subnet" },
      { id: "pip>nsg", sourceId: "pip", targetId: "nsg" },
    ],
    errorCount: 0,
  };

  function sameGraph(current) {
    if (!current) return false;
    if (current.nodes.length !== graph.nodes.length || current.edges.length !== graph.edges.length) return false;
    return graph.nodes.every(function (node) { return current.nodes.some(function (currentNode) { return currentNode.id === node.id; }); }) &&
      graph.edges.every(function (edge) { return current.edges.some(function (currentEdge) { return currentEdge.id === edge.id; }); });
  }

  function graphUpdatePatches(current) {
    if (sameGraph(current)) return [];
    return [
      { op: "clearGraph" },
      { op: "setErrorCount", errorCount: graph.errorCount },
    ].concat(
      graph.nodes.map(function (node) { return { op: "addNode", node: node }; }),
      graph.edges.map(function (edge) { return { op: "addEdge", edge: edge }; })
    );
  }

  function graphLayoutPatches() {
    var positions = {
      vnet: { x: 0, y: 0 },
      subnet: { x: 190, y: 90 },
      nsg: { x: 380, y: 180 },
      pip: { x: 570, y: 270 },
    };

    return graph.nodes.map(function (node) {
      return { op: "setNodeLayout", nodeId: node.id, layout: positions[node.id] };
    }).concat([{ op: "setGraphBounds", bounds: { width: 760, height: 420 } }]);
  }

  window.acquireVsCodeApi = function () {
    return {
      postMessage: function (msg) {
        console.log("[fake vscode-api] postMessage:", msg);
        if (msg && msg.method === "ready") {
          setTimeout(function () {
            window.postMessage({
              method: "documentDidChange",
              params: { documentUri: "file:///main.bicep" },
            }, "*");
          }, 100);
        } else if (msg && msg.method === "getGraphUpdate") {
          window.postMessage({
            id: msg.id,
            result: { patches: graphUpdatePatches(msg.params && msg.params.current) },
          }, "*");
        } else if (msg && msg.method === "getGraphLayout") {
          window.postMessage({
            id: msg.id,
            result: { status: "ok", patches: graphLayoutPatches() },
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
    rollupOptions: {
      output: {
        entryFileNames: `[name].js`,
        chunkFileNames: `chunks/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
  test: {
    // Unit tests live next to the source under `src`. Playwright e2e specs in `e2e/`
    // are run by Playwright, not Vitest, so keep them out of test discovery.
    include: ["src/**/*.test.{ts,tsx}"],
  },
});
