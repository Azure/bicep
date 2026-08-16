// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createExtensionHostTestRunner, TestFrameworkRunner } from "../test-e2e/test-runner";

describe("createExtensionHostTestRunner", () => {
  test("resolves when the framework run succeeds", async () => {
    const runner = createExtensionHostTestRunner(createFrameworkRunner(0, 0));

    await expect(runner()).resolves.toBeUndefined();
  });

  test.each([
    [1, 0, "1 suite(s), 0 test(s)"],
    [0, 2, "0 suite(s), 2 test(s)"],
  ])("rejects when the framework reports failures", async (failedSuites, failedTests, message) => {
    const runner = createExtensionHostTestRunner(createFrameworkRunner(failedSuites, failedTests));

    await expect(runner()).rejects.toThrow(message);
  });
});

function createFrameworkRunner(failedSuiteCount: number, failedTestCount: number): TestFrameworkRunner {
  return {
    run: async () => ({ failedSuiteCount, failedTestCount, totalSuiteCount: 1 }),
  };
}
