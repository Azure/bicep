// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable jest/no-standalone-expect */
import spawn from "cross-spawn";

import { bicepCli } from "./fs";

class StdoutAssertionBuilder {
  constructor(private readonly stdout: string) {}

  withStdout(expectedStdout: string | RegExp): void {
    if (typeof expectedStdout === "string") {
      expect(this.stdout).toBe(expectedStdout);
    } else {
      expect(this.stdout).toMatch(expectedStdout);
    }
  }

  withEmptyStdout(): void {
    expect(this.stdout).toHaveLength(0);
  }

  withNonEmptyStdout(): void {
    expect(this.stdout.length).toBeGreaterThan(0);
  }
}

class StderrAssertionBuilder {
  constructor(private readonly stderr: string) {}

  withStderr(expectedStderr: string | RegExp): void {
    if (typeof expectedStderr === "string") {
      expect(this.stderr).toBe(expectedStderr);
    } else {
      expect(this.stderr).toMatch(expectedStderr);
    }
  }

  withNonEmptyStderr(): void {
    expect(this.stderr.length).toBeGreaterThan(0);
  }
}

class BicepCommandTestRunner {
  constructor(private readonly args: string[]) {}

  shouldSucceed(): StdoutAssertionBuilder {
    const result = this.runCommand();

    if (result.stderr.length > 0) {
      console.error(result.stderr);
    }

    expect(result.status).toBe(0);

    return new StdoutAssertionBuilder(result.stdout);
  }

  shouldFail(): StderrAssertionBuilder {
    const result = this.runCommand();
    expect(result.status).toBe(1);

    return new StderrAssertionBuilder(result.stderr);
  }

  private runCommand() {
    return spawn.sync(bicepCli, this.args, {
      stdio: "pipe",
      encoding: "utf-8",
      env: { ...process.env },
    });
  }
}

export function invokingBicepCommand(
  ...args: string[]
): BicepCommandTestRunner {
  return new BicepCommandTestRunner(args);
}
