// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationTarget, WorkspaceConfiguration } from "vscode";
import { SuppressedWarningsManager } from "./suppressed-warnings";

function createConfiguration(initialValue?: unknown): Pick<WorkspaceConfiguration, "get" | "update"> {
  let value = initialValue;

  function get<T>(): T | undefined;
  function get<T>(_section: string, defaultValue: T): T;
  function get<T>(_section?: string, defaultValue?: T): T | undefined {
    return value === undefined ? defaultValue : (value as T);
  }

  return {
    get,
    update: async (_section, updatedValue, target) => {
      expect(target).toBe(ConfigurationTarget.Global);
      value = updatedValue;
    },
  };
}

describe("SuppressedWarningsManager", () => {
  it("doesn't suppress warnings by default", () => {
    const config = createConfiguration();
    const manager = new SuppressedWarningsManager(() => config);

    expect(manager.isWarningSuppressed("test")).toBe(false);
  });

  it("suppresses a requested warning", async () => {
    const config = createConfiguration();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBe(true);
  });

  it("resets a suppressed warning", async () => {
    const config = createConfiguration();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBe(true);

    await manager.resetWarning("test");
    expect(manager.isWarningSuppressed("test")).toBe(false);
  });

  it("recovers from invalid configuration", async () => {
    const config = createConfiguration(123456);
    const manager = new SuppressedWarningsManager(() => config);

    expect(manager.isWarningSuppressed("test")).toBe(false);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBe(true);
  });
});
