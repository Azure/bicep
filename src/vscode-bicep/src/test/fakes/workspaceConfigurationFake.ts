// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ConfigurationTarget, WorkspaceConfiguration } from "vscode";
import { sleep } from "../../utils/time";

export class WorkspaceConfigurationFake implements WorkspaceConfiguration {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private dictionary = new Map<string, any>();

  public get<T>(section: string): T | undefined;
  public get<T>(section: string, defaultValue: T): T;
  public get<T>(section: unknown, defaultValue?: unknown): T | T | undefined {
    if (typeof section === "string") {
      return this.has(section) ? this.dictionary.get(section) : defaultValue;
    }

    throw new Error(`Unsupported key type: ${typeof section}`);
  }

  public has(section: string): boolean {
    return this.dictionary.has(section);
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public inspect<T>(section: string):
    | {
        key: string;
        defaultValue?: T | undefined;
        globalValue?: T | undefined;
        workspaceValue?: T | undefined;
        workspaceFolderValue?: T | undefined;
        defaultLanguageValue?: T | undefined;
        globalLanguageValue?: T | undefined;
        workspaceLanguageValue?: T | undefined;
        workspaceFolderLanguageValue?: T | undefined;
        languageIds?: string[] | undefined;
      }
    | undefined {
    throw new Error("Method not implemented.");
  }

  public async update(
    section: string,
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    value: any,
    configurationTarget?: boolean | ConfigurationTarget | null | undefined,
    overrideInLanguage?: boolean | undefined,
  ): Promise<void> {
    if (
      configurationTarget !== ConfigurationTarget.Global &&
      configurationTarget !== true
    ) {
      throw new Error(
        "Functionality not implemented: WorkspaceConfigurationFake currently only supports global configuration target",
      );
    }

    if (overrideInLanguage) {
      throw new Error("Functionality not implemented: overrideInLanguage");
    }

    await sleep(1);
    this.dictionary.set(section, value);
  }
}
