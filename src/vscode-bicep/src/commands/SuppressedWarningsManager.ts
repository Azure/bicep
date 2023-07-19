// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationTarget, WorkspaceConfiguration } from "vscode";
import { getBicepConfiguration } from "../language/getBicepConfiguration";

export class SuppressedWarningsManager {
  public static readonly suppressedWarningsConfigurationKey =
    "suppressedWarnings";

  public static readonly keys = {
    decompileOnPasteWarning: "Decompile on paste",
  };

  public constructor(
    private readonly provideBicepConfiguration: () => WorkspaceConfiguration = getBicepConfiguration, // override for unit testing
  ) {
    // noop
  }

  public isWarningSuppressed(key: string): boolean {
    const suppressedWarnings: string[] = this.getSuppressedWarnings();
    return suppressedWarnings.includes(key);
  }

  public async suppressWarning(key: string): Promise<void> {
    const suppressedWarnings: string[] = this.getSuppressedWarnings();
    if (!suppressedWarnings.includes(key)) {
      suppressedWarnings.push(key);
    }

    await this.provideBicepConfiguration().update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      suppressedWarnings,
      ConfigurationTarget.Global,
    );
  }

  public async resetWarning(key: string): Promise<void> {
    let suppressedWarnings: string[] = this.getSuppressedWarnings();
    suppressedWarnings = suppressedWarnings.filter((k) => k !== key);

    await this.provideBicepConfiguration().update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      suppressedWarnings,
      ConfigurationTarget.Global,
    );
  }

  private getSuppressedWarnings(): string[] {
    const currentSuppressedKeys = this.provideBicepConfiguration().get(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
    );

    return Array.isArray(currentSuppressedKeys) ? currentSuppressedKeys : [];
  }
}
