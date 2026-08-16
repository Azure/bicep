// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Reporter, VerboseReporter as VitestVerboseReporter } from "vitest/node" with {
  "resolution-mode": "import",
};

import { Writable } from "stream";
import { format } from "util";
import { TestRunResult } from "../test-runner";

export interface ExtensionHostReporter extends Reporter {
  readonly result: TestRunResult;
}

export function createExtensionHostErrorStream(writeError: (value: string) => void): Writable {
  return new Writable({
    write(chunk, _encoding, callback) {
      const content = String(chunk);
      if (!isMissingAzExtUtilsSourceMapWarning("stderr", content)) {
        writeError(content.replace(/\r?\n$/, ""));
      }
      callback();
    },
  });
}

// Use Vitest's own formatter for symbols, colors, durations, failures, and summary details. Only the
// output destination and console-log timing differ because Vitest runs inside VS Code's Extension Host.
export function createExtensionHostReporter(
  ReporterBase: typeof VitestVerboseReporter,
  writeOutput: (value: string) => void,
  writeError: (value: string) => void,
): ExtensionHostReporter {
  return new (class extends ReporterBase implements ExtensionHostReporter {
    private failedSuiteCount = 0;
    private failedTestCount = 0;
    private totalSuiteCount = 0;
    private readonly consoleLogs: Array<Parameters<NonNullable<Reporter["onUserConsoleLog"]>>[0]> = [];

    public constructor() {
      super({ isTTY: false });
    }

    public get result(): TestRunResult {
      return {
        failedSuiteCount: this.failedSuiteCount,
        failedTestCount: this.failedTestCount,
        totalSuiteCount: this.totalSuiteCount,
      };
    }

    public override log(...messages: unknown[]): void {
      writeOutput(format(...messages));
    }

    public override error(...messages: unknown[]): void {
      writeError(format(...messages));
    }

    public override onUserConsoleLog(log: Parameters<NonNullable<Reporter["onUserConsoleLog"]>>[0]): void {
      if (
        !isMissingAzExtUtilsSourceMapWarning(log.type, log.content) &&
        log.taskId &&
        log.taskId !== "__vitest__unknown_test__" &&
        log.content.trim().length > 0
      ) {
        this.consoleLogs.push(log);
      }
    }

    public override onTestCaseResult(testCase: Parameters<NonNullable<Reporter["onTestCaseResult"]>>[0]): void {
      if (testCase.result().state === "failed") {
        this.failedTestCount++;
      }

      super.onTestCaseResult(testCase);
    }

    public override onTestRunEnd(
      testModules: Parameters<NonNullable<Reporter["onTestRunEnd"]>>[0],
      unhandledErrors: Parameters<NonNullable<Reporter["onTestRunEnd"]>>[1],
      reason: Parameters<NonNullable<Reporter["onTestRunEnd"]>>[2],
    ): void {
      this.failedSuiteCount =
        testModules.filter((module) => module.state() === "failed").length + unhandledErrors.length;
      this.totalSuiteCount = testModules.length;

      if (reason === "failed" && this.failedSuiteCount === 0 && this.failedTestCount === 0) {
        this.failedSuiteCount = 1;
      }

      this.flushConsoleLogs();
      super.onTestRunEnd(testModules, unhandledErrors, reason);
    }

    private flushConsoleLogs(): void {
      const testLogs = this.consoleLogs
        .slice()
        .sort((left, right) => left.time - right.time)
        .flatMap((log) => {
          const task = log.taskId ? this.ctx.state.idMap.get(log.taskId) : undefined;
          return task ? [{ log, task }] : [];
        });

      if (testLogs.length === 0) {
        return;
      }

      this.log();
      this.log("Console output:");
      for (const { log, task } of testLogs) {
        const context = this.getFullName(task, " > ");
        const prefix = log.type === "stderr" ? "stderr" : "stdout";
        this.log(`${prefix} | ${context}\n${log.content.trimEnd()}`);
      }
    }
  })();
}

export function isMissingAzExtUtilsSourceMapWarning(type: string, content: string): boolean {
  if (type !== "stderr") {
    return false;
  }

  const normalizedContent = content.replaceAll("\\", "/");
  const packagePath = "/node_modules/@microsoft/vscode-azext-utils/dist/esm/";
  const enoentIndex = normalizedContent.indexOf("Error: ENOENT: no such file or directory");

  return (
    normalizedContent.includes("Failed to load source map for ") &&
    normalizedContent.includes(packagePath) &&
    enoentIndex >= 0 &&
    normalizedContent.indexOf(packagePath, enoentIndex) > enoentIndex &&
    normalizedContent.indexOf(".js.map", enoentIndex) > enoentIndex
  );
}
