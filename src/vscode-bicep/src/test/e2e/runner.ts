// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import { runCLI } from "jest";

// https://github.com/microsoft/vscode/blob/87dd7d6a9c2f5fcc2bf3abe555fee79fbbde34bb/src/vs/workbench/api/common/extHostExtensionService.ts#L44
export type TestRunner = () => Promise<void>;

export function createTestRunner(configFile: string): TestRunner {
  return async () => {
    const workspaceRoot = path.resolve(__dirname, "../../..");
    const config = require(path.join(workspaceRoot, configFile)); // eslint-disable-line @typescript-eslint/no-var-requires

    const { results } = await runCLI({ _: [], $0: "", ...config }, [
      workspaceRoot,
    ]);

    if (results.numFailedTestSuites > 0 || results.numFailedTests > 0) {
      process.exit(1);
    }
  };
}
