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
): Promise<[T, string]> {
  const originalWrites = writeStreams.map((ws) => ws.write);
  const outputs: string[] = [];

  try {
    const write = (chunk: Uint8Array | string) => {
      if (typeof chunk === "string" && chunk.length > 0) {
        outputs.push(chunk);
      }
      return true;
    };
    writeStreams.forEach((ws) => (ws.write = write));
    return [await fn(), outputs.join("")];
  } finally {
    writeStreams.forEach((ws, i) => (ws.write = originalWrites[i]));
  }
}

export function createTestRunner(configFile: string): TestRunner {
  return async () => {
    const workspaceRoot = path.resolve(__dirname, "../../..");
    const config = require(path.join(workspaceRoot, configFile)); // eslint-disable-line @typescript-eslint/no-var-requires
    const [{ results }, outputs] = await captureWriteStreams(
      async () => await runCLI({ _: [], $0: "", ...config }, [workspaceRoot])
    );

    console.log(outputs);

    if (results.numFailedTestSuites > 0 || results.numFailedTests > 0) {
      throw new Error(`Tests failed.`);
    }
  };
}
