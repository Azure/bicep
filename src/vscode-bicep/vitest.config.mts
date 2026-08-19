// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { fileURLToPath } from "url";
import { defineConfig } from "vitest/config";

const configDirectory = path.dirname(fileURLToPath(import.meta.url));
const missingAzExtUtilsSourceMapWarning =
  /Failed to load source map for .*[/\\]node_modules[/\\]@microsoft[/\\]vscode-azext-utils[/\\]dist[/\\]esm[/\\].*\.js\.[\s\S]*ENOENT:[\s\S]*\.js\.map/;

export default defineConfig({
  plugins: [
    {
      name: "suppress-missing-azext-utils-source-maps",
      configResolved(config) {
        const warn = config.logger.warn.bind(config.logger);
        config.logger.warn = (message, options) => {
          if (!missingAzExtUtilsSourceMapWarning.test(message)) {
            warn(message, options);
          }
        };
      },
    },
  ],
  resolve: {
    alias: {
      vscode: path.resolve(configDirectory, "tests/setup/unit.ts"),
    },
  },
  test: {
    globals: true,
    include: ["src/**/__tests__/**/*.test.ts", "tests/e2e/**/__tests__/**/*.test.ts", "package.test.ts"],
    setupFiles: ["tests/setup/unit.ts"],
    coverage: {
      enabled: true,
      provider: "v8",
      include: ["src/**/*.ts"],
      exclude: ["src/**/__tests__/**", "src/features/visualization/**", "**/.svg/**"],
      reporter: ["clover", "json", "lcov", "text"],
    },
  },
});
