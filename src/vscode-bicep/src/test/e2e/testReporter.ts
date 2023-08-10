// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Context, Reporter, Test } from "@jest/reporters";
import { AggregatedResult, TestResult } from "@jest/test-result";

export default class TestReporter
  implements Pick<Reporter, "onTestResult" | "onRunComplete">
{
  onTestResult(test: Test, { testResults }: TestResult): void {
    for (const testResult of testResults) {
      switch (testResult.status) {
        case "passed":
          console.log(`✓ ${testResult.fullName}`);
          break;
        case "skipped":
        case "pending":
        case "todo":
        case "disabled":
          console.log(`* ${testResult.fullName}`);
          break;
        case "failed":
          console.log(`✘ ${testResult.fullName}`);
          for (const failureMessage of testResult.failureMessages) {
            console.log(failureMessage);
          }
          break;
        default:
          console.log(`(${testResult.status}) ${testResult.fullName}`);
      }
    }
  }

  onRunComplete(contexts: Set<Context>, results: AggregatedResult): void {
    console.log("");
    console.log(
      `Test Suites: ${results.numPassedTestSuites} passed, ${results.numTotalTestSuites} total`,
    );
    console.log(
      `Tests:       ${results.numPassedTestSuites} passed, ${results.numTotalTestSuites} total`,
    );
  }
}
