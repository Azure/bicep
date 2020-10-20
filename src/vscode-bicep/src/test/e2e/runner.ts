// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";

// eslint-disable-next-line jest/no-jest-import
import { runCLI } from "jest";
import { WriteStream } from "tty";

// https://github.com/microsoft/vscode/blob/87dd7d6a9c2f5fcc2bf3abe555fee79fbbde34bb/src/vs/workbench/api/common/extHostExtensionService.ts#L44
export type TestRunner = () => Promise<void>;

async function captureWriteStreams<T>(
  fn: () => Promise<T>,
  writeStreams: WriteStream[] = [process.stdout, process.stderr]
): Promise<T> {
  const originalWrites = writeStreams.map((ws) => ws.write);

  try {
    const write = (chunk: Uint8Array | string) => {
      if (typeof chunk === "string") {
        console.log(chunk.endsWith("\n") ? chunk.slice(0, -1) : chunk);
      }
      return true;
    };
    writeStreams.forEach((ws) => (ws.write = write));
    return await fn();
  } finally {
    writeStreams.forEach((ws, i) => (ws.write = originalWrites[i]));
  }
}

export function createTestRunner(configFile: string): TestRunner {
  return async () => {
    const workspaceRoot = path.resolve(__dirname, "../../..");
    const config = require(path.join(workspaceRoot, configFile)); // eslint-disable-line @typescript-eslint/no-var-requires
    const { results } = await captureWriteStreams(
      async () => await runCLI({ _: [], $0: "", ...config }, [workspaceRoot])
    );

    if (results.numFailedTestSuites > 0 || results.numFailedTests > 0) {
      throw new Error(`Tests failed.`);
    }
  };
}
