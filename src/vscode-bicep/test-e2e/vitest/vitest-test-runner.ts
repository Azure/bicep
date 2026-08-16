// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import path from "path";
import { TestFrameworkRunner, TestRunResult } from "../test-runner";
import { createExtensionHostPool } from "./extension-host-pool";
import { createExtensionHostErrorStream, createExtensionHostReporter } from "./extension-host-reporter";

export class VitestTestRunner implements TestFrameworkRunner {
  public async run(): Promise<TestRunResult> {
    const workspaceRoot = path.resolve(__dirname, "../../..");
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
      const { reporter, vitest } = await withoutNavigator(async () => {
        const { startVitest, VerboseReporter } = await import("vitest/node");
        const reporter = createExtensionHostReporter(VerboseReporter, writeOutput, writeError);
        const errorStream = createExtensionHostErrorStream(writeError);
        // Vitest is the controller. createExtensionHostPool supplies its worker side in this same
        // process; a normal threads/forks pool would lose access to the extension-host `vscode` API.
        const vitest = await startVitest(
          "test",
          [],
          {
            config: false,
            root: workspaceRoot,
            watch: false,
            globals: true,
            color: true,
            include: ["test-e2e/**/*.test.ts"],
            setupFiles: [path.join(workspaceRoot, "test-e2e/vitest/setup.ts")],
            pool: createExtensionHostPool(),
            // VS Code tests share editors, settings, and extension state, so run one file at a time.
            fileParallelism: false,
            isolate: false,
            testTimeout: 60_000,
            hookTimeout: 60_000,
            reporters: [reporter],
          },
          undefined,
          { stderr: errorStream },
        );
        return { reporter, vitest };
      });

      if (!vitest) {
        throw new Error("Vitest failed to start.");
      }

      const modules = vitest.state.getTestModules();
      if (modules.length === 0) {
        throw new Error("Vitest did not discover any E2E test modules.");
      }

      return reporter.result;
    } finally {
      globalThis.__bicepVscodeApi = previousVscodeApi;
    }
  }
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
