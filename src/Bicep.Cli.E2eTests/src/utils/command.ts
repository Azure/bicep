// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import spawn from "cross-spawn";
import { expect } from "vitest";
import { bicepCli } from "./fs";
import { EnvironmentOverrides } from "./liveTestEnvironments";
import { logStdErr } from "./log";

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
  private environmentOverrides: EnvironmentOverrides = {};

  constructor(private readonly args: string[]) {}

  withEnvironmentOverrides(environmentOverrides: EnvironmentOverrides): BicepCommandTestRunner {
    this.environmentOverrides = environmentOverrides;
    return this;
  }

  shouldSucceed(): StdoutAssertionBuilder {
    const result = this.runCommand();
    logStdErr(result.stderr);

    expect(result.status).toBe(0);

    return new StdoutAssertionBuilder(result.stdout);
  }

  shouldFail(): StderrAssertionBuilder {
    const result = this.runCommand();

    expect(result.status).toBe(1);

    return new StderrAssertionBuilder(result.stderr);
  }

  private runCommand() {
    const result = spawn.sync(bicepCli, this.args, {
      stdio: "pipe",
      encoding: "utf-8",
      // overrides take precedence over inherited env vars
      env: { ...process.env, ...this.environmentOverrides },
    });

    expect(result).toBeTruthy();

    if (result.status == null) {
      throw new Error(`Process terminated prematurely. result = ${JSON.stringify(result)}`);
    }

    return result;
  }
}

export function invokingBicepCommand(...args: string[]): BicepCommandTestRunner {
  return new BicepCommandTestRunner(args);
}
