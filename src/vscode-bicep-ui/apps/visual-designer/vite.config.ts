// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import fs from "fs";
import path from "path";
import react from "@vitejs/plugin-react";
import { defineConfig, type Plugin } from "vite";

/**
 * Inject a fake `acquireVsCodeApi` with sample graph data only
 * during `vite preview`.  This lets us verify the production build
 * locally without polluting the actual bundle.
 */
function previewMock(): Plugin {
  const mockScript = `
<script>
  window.acquireVsCodeApi = function () {
    return {
      postMessage: function (msg) {
        console.log("[fake vscode-api] postMessage:", msg);
        if (msg && msg.method === "ready") {
          setTimeout(function () {
            window.postMessage({
              method: "deploymentGraph",
              params: {
                documentPath: "file:///main.bicep",
                deploymentGraph: {
                  nodes: [
                    { id: "vnet", type: "Microsoft.Network/virtualNetworks", isCollection: false, range: { start: { line: 0, character: 0 }, end: { line: 5, character: 1 } }, hasChildren: false, hasError: false, filePath: "file:///main.bicep" },
                    { id: "subnet", type: "Microsoft.Network/virtualNetworks/subnets", isCollection: false, range: { start: { line: 7, character: 0 }, end: { line: 12, character: 1 } }, hasChildren: false, hasError: false, filePath: "file:///main.bicep" },
                    { id: "nsg", type: "Microsoft.Network/networkSecurityGroups", isCollection: false, range: { start: { line: 14, character: 0 }, end: { line: 19, character: 1 } }, hasChildren: false, hasError: false, filePath: "file:///main.bicep" },
                    { id: "pip", type: "Microsoft.Network/publicIPAddresses", isCollection: true, range: { start: { line: 21, character: 0 }, end: { line: 26, character: 1 } }, hasChildren: false, hasError: false, filePath: "file:///main.bicep" },
                  ],
                  edges: [
                    { sourceId: "subnet", targetId: "vnet" },
                    { sourceId: "nsg", targetId: "subnet" },
                    { sourceId: "pip", targetId: "nsg" },
                  ],
                  errorCount: 0,
                },
              },
            });
          }, 100);
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
});
