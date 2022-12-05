// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationTarget } from "vscode";
import { SuppressedWarningsManager } from "../../commands/SuppressedWarningsManager";
import { WorkspaceConfigurationFake } from "../fakes/workspaceConfigurationFake";

describe("suppressedWarningsManager", () => {
  it("should not suppress by default", () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    expect(manager.isWarningSuppressed("test")).toBeFalsy();
  });

  it("should suppress when requested", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();
  });

  it("should reset when requested", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();

    await manager.resetWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeFalsy();
  });

  it("should handle bad configuration", async () => {
    const config = new WorkspaceConfigurationFake();
    const manager = new SuppressedWarningsManager(() => config);

    await config.update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      123456,
      ConfigurationTarget.Global
    );

    expect(manager.isWarningSuppressed("test")).toBeFalsy();

    await manager.suppressWarning("test");
    expect(manager.isWarningSuppressed("test")).toBeTruthy();
  });
});
