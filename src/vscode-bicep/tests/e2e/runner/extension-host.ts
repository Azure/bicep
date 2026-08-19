// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { Writable } from "stream";
import { createExtensionHostPool } from "./pool";

const missingAzExtUtilsSourceMapWarning =
  /Failed to load source map for .*[/\\]node_modules[/\\]@microsoft[/\\]vscode-azext-utils[/\\]dist[/\\]esm[/\\].*\.js\.[\s\S]*ENOENT:[\s\S]*\.js\.map/;

// VS Code loads this module inside the Extension Host and invokes its exported run function.
export async function run(): Promise<void> {
  const workspaceRoot = path.resolve(__dirname, "../../../..");
  // This require succeeds only inside the extension host. Store the API on globalThis because
  // Vitest evaluates setup/test modules through Vite's module runner, not this CommonJS module graph.
  const vscodeApi = require("vscode") as typeof import("vscode"); // eslint-disable-line @typescript-eslint/no-require-imports
  const previousVscodeApi = globalThis.__bicepVscodeApi;
  globalThis.__bicepVscodeApi = vscodeApi;

  try {
    const extension = vscodeApi.extensions.getExtension("ms-azuretools.vscode-bicep");
    if (!extension) {
      throw new Error("Bicep extension was not registered in the Extension Development Host.");
    }
    await extension.activate();

    // Bind before startVitest replaces console methods. The reporter can later flush through this
    // original Extension Host console while test console calls remain intercepted and buffered.
    const writeOutput = console.log.bind(console);
    const writeError = console.error.bind(console);
    const vitest = await withoutNavigator(async () => {
      const { startVitest } = await import("vitest/node");
      return await startVitest(
        "test",
        [],
        {
          config: false,
          root: workspaceRoot,
          watch: false,
          globals: true,
          color: true,
          include: ["tests/e2e/*.test.ts"],
          setupFiles: [path.join(workspaceRoot, "tests/e2e/runner/setup.ts")],
          pool: createExtensionHostPool(),
          // VS Code tests share editors, settings, and extension state, so run one file at a time.
          fileParallelism: false,
          testTimeout: 60_000,
          hookTimeout: 60_000,
          reporters: ["verbose"],
        },
        undefined,
        {
          stdout: createExtensionHostStream(writeOutput),
          stderr: createExtensionHostStream(writeError, (content) => !missingAzExtUtilsSourceMapWarning.test(content)),
        },
      );
    });

    const modules = vitest.state.getTestModules();
    if (modules.length === 0) {
      throw new Error("Vitest did not discover any E2E test modules.");
    }

    const failedModuleCount = modules.filter((module) => !module.ok()).length;
    const unhandledErrorCount = vitest.state.getUnhandledErrors().length;
    if (failedModuleCount > 0 || unhandledErrorCount > 0) {
      throw new Error(`E2E tests failed: ${failedModuleCount} module(s), ${unhandledErrorCount} unhandled error(s).`);
    }
  } catch (error) {
    console.error("E2E test runner failed.", error);
    throw error;
  } finally {
    globalThis.__bicepVscodeApi = previousVscodeApi;
  }
}

function createExtensionHostStream(
  write: (value: string) => void,
  shouldWrite: (content: string) => boolean = () => true,
): Writable {
  return new Writable({
    write(chunk, _encoding, callback) {
      const content = String(chunk);
      if (shouldWrite(content)) {
        write(content.replace(/\r?\n$/, ""));
      }
      callback();
    },
  });
}

async function withoutNavigator<T>(action: () => Promise<T>): Promise<T> {
  // VS Code currently exposes navigator through a migration getter that deliberately throws when
  // old Node code probes it. Vite's picomatch dependency performs that probe during startup.
  // Temporarily shadow it so picomatch falls back to process.platform, then restore the host exactly.
  const descriptor = Object.getOwnPropertyDescriptor(globalThis, "navigator");
  Object.defineProperty(globalThis, "navigator", { configurable: true, value: undefined });

  try {
    return await action();
  } finally {
    if (descriptor) {
      Object.defineProperty(globalThis, "navigator", descriptor);
    } else {
      delete (globalThis as { navigator?: unknown }).navigator;
    }
  }
}
