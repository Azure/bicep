// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { cpSync, mkdirSync, readdirSync, rmSync } from "fs";
import { builtinModules } from "module";
import path from "path";
import { fileURLToPath } from "url";
import { defineConfig, Plugin } from "vite";

const configDirectory = path.dirname(fileURLToPath(import.meta.url));
const outputPath = path.resolve(configDirectory, "out");
// Some bundled CommonJS dependencies load the VS Code API with require().
const commonJsRequireShim =
  'import { createRequire as __createRequire } from "node:module";\nconst require = __createRequire(import.meta.url);';
const directoryCopies = [
  {
    source: path.resolve(configDirectory, "../vscode-bicep-ui/apps/deploy-pane/dist"),
    destination: path.resolve(outputPath, "deploy-pane"),
  },
  {
    source: path.resolve(configDirectory, "../vscode-bicep-ui/apps/visual-designer/dist"),
    destination: path.resolve(outputPath, "visual-designer"),
  },
];
const fileCopies = [
  {
    source: path.resolve(configDirectory, "../textmate/bicep.tmlanguage"),
    destination: path.resolve(configDirectory, "resources/language/bicep.tmlanguage"),
  },
  {
    source: path.resolve(configDirectory, "../textmate/language-configuration.json"),
    destination: path.resolve(configDirectory, "resources/language/language-configuration.json"),
  },
];

export default defineConfig(({ mode }) => ({
  build: {
    emptyOutDir: true,
    lib: {
      entry: path.resolve(configDirectory, "src/extension.ts"),
      formats: ["es"],
    },
    minify: mode === "production" ? "oxc" : false,
    outDir: outputPath,
    rolldownOptions: {
      external: ["vscode", ...builtinModules, ...builtinModules.map((moduleName) => `node:${moduleName}`)],
      output: {
        banner: commonJsRequireShim,
        codeSplitting: false,
        entryFileNames: "extension.js",
        keepNames: true,
      },
    },
    sourcemap: true,
    target: "es2022",
  },
  plugins: [copyExtensionAssets()],
  resolve: {
    conditions: ["node", "import"],
  },
}));

function copyExtensionAssets(): Plugin {
  return {
    name: "copy-extension-assets",
    buildStart() {
      for (const { source } of directoryCopies) {
        for (const filePath of getFiles(source)) {
          if (path.basename(filePath) !== "index.html") {
            this.addWatchFile(filePath);
          }
        }
      }

      for (const { source } of fileCopies) {
        this.addWatchFile(source);
      }
    },
    writeBundle() {
      for (const { source, destination } of directoryCopies) {
        rmSync(destination, { force: true, recursive: true });
        cpSync(source, destination, {
          filter: (filePath) => path.basename(filePath) !== "index.html",
          recursive: true,
        });
      }

      for (const { source, destination } of fileCopies) {
        mkdirSync(path.dirname(destination), { recursive: true });
        cpSync(source, destination);
      }
    },
  };
}

function getFiles(directoryPath: string): string[] {
  return readdirSync(directoryPath, { withFileTypes: true }).flatMap((entry) => {
    const entryPath = path.join(directoryPath, entry.name);
    return entry.isDirectory() ? getFiles(entryPath) : [entryPath];
  });
}
