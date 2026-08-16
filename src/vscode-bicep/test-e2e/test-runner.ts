// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export interface TestRunResult {
  failedSuiteCount: number;
  failedTestCount: number;
  totalSuiteCount: number;
}

// Framework adapters hide Jest/Vitest-specific APIs behind counts that the VS Code entry point understands.
export interface TestFrameworkRunner {
  run(): Promise<TestRunResult>;
}

// VS Code invokes this exported shape inside the extension host.
export type ExtensionHostTestRunner = () => Promise<void>;

export function createExtensionHostTestRunner(frameworkRunner: TestFrameworkRunner): ExtensionHostTestRunner {
  return async () => {
    try {
      const result = await frameworkRunner.run();
      if (result.failedSuiteCount > 0 || result.failedTestCount > 0) {
        // Throwing rejects the run() promise. @vscode/test-electron converts that rejection to exit code 1.
        throw new Error(`E2E tests failed: ${result.failedSuiteCount} suite(s), ${result.failedTestCount} test(s).`);
      }
    } catch (error) {
      console.error("E2E test runner failed.", error);
      throw error;
    }
  };
}
