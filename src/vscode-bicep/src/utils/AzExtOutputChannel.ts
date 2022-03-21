// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  OutputChannel,
  ViewColumn,
  window,
  workspace,
  WorkspaceConfiguration,
} from "vscode";

export function createAzExtOutputChannel(
  name: string,
  extensionPrefix: string
): IAzExtOutputChannel {
  return new AzExtOutputChannel(name, extensionPrefix);
}

/**
 * Wrapper for vscode.OutputChannel that handles AzureExtension behavior for outputting messages
 */
export interface IAzExtOutputChannel extends OutputChannel {
  /**
   * appendLog adds the current timestamps to all messages
   * @param value The message to be printed
   * @param options.resourceName The name of the resource. If provided, the resource name will be prefixed to the message
   * @param options.date The date to prepend before the message, otherwise it defaults to Date.now()
   */
  appendLog(
    value: string,
    options?: { resourceName?: string; date?: Date }
  ): void;
}

class AzExtOutputChannel implements IAzExtOutputChannel {
  public readonly name: string;
  public readonly extensionPrefix: string;
  private _outputChannel: OutputChannel;

  constructor(name: string, extensionPrefix: string) {
    this.name = name;
    this.extensionPrefix = extensionPrefix;
    this._outputChannel = window.createOutputChannel(this.name);
  }

  public replace(value: string): void {
    this._outputChannel.replace(value);
  }

  public append(value: string): void {
    this._outputChannel.append(value);
  }

  public appendLine(value: string): void {
    const updatedValue = this.removePropertiesWithPossibleUserInfo(value);

    this._outputChannel.appendLine(updatedValue);
  }

  private removePropertiesWithPossibleUserInfo(value: string) {
    const deployParamsPattern = new RegExp(
      '.*"token":\\s*(?<token>.*)\\s*.*"expiresOnTimestamp":\\s*(?<expiresOnTimestamp>.*)\\s*.*'
    );
    const matches = deployParamsPattern.exec(value);

    if (matches != null) {
      const groups = matches.groups;

      if (groups != null) {
        const token = groups["token"];
        const expiresOnTimestamp = groups["expiresOnTimestamp"];

        let updatedValue = value.replace(token, '"<REDACTED: token>"');
        updatedValue = updatedValue.replace(
          expiresOnTimestamp,
          '"<REDACTED: expiresOnTimestamp>"'
        );

        return updatedValue;
      }
    }

    return value;
  }

  public appendLog(
    value: string,
    options?: { resourceName?: string; date?: Date }
  ): void {
    const enableOutputTimestampsSetting = "enableOutputTimestamps";
    const projectConfiguration: WorkspaceConfiguration =
      workspace.getConfiguration(this.extensionPrefix);
    const result: boolean | undefined = projectConfiguration.get<boolean>(
      enableOutputTimestampsSetting
    );

    if (!result) {
      this.appendLine(value);
    } else {
      options ||= {};
      const date: Date = options.date || new Date();
      this.appendLine(
        `${date.toLocaleTimeString()}${
          options.resourceName ? " ".concat(options.resourceName) : ""
        }: ${value}`
      );
    }
  }

  public clear(): void {
    this._outputChannel.clear();
  }

  public show(preserveFocus?: boolean | undefined): void;
  public show(
    column?: ViewColumn | undefined,
    preserveFocus?: boolean | undefined
  ): void;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public show(_column?: any, preserveFocus?: boolean | undefined): void {
    this._outputChannel.show(preserveFocus);
  }

  public hide(): void {
    this._outputChannel.hide();
  }

  public dispose(): void {
    this._outputChannel.dispose();
  }
}
