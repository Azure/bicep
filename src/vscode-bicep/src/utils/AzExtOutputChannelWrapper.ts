// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { OutputChannel, ViewColumn } from "vscode";

import {
  createAzExtOutputChannel,
  IAzExtOutputChannel,
} from "@microsoft/vscode-azext-utils";

export function createOutputChannel(
  name: string,
  extensionPrefix: string
): IAzExtOutputChannel {
  const azExtOutputChannel = createAzExtOutputChannel(name, extensionPrefix);
  return new AzExtOutputChannelWrapper(name, azExtOutputChannel);
}

/**
 * Wrapper for vscode.OutputChannel that handles AzureExtension behavior for outputting messages
 */
export interface IOutputChannel extends OutputChannel {
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

class AzExtOutputChannelWrapper implements IOutputChannel {
  private _azExtOutputChannel: IAzExtOutputChannel;
  public readonly name: string;

  constructor(name: string, azExtOutputChannel: IAzExtOutputChannel) {
    this._azExtOutputChannel = azExtOutputChannel;
    this.name = name;
  }
  appendLog(
    value: string,
    options?: { resourceName?: string | undefined; date?: Date | undefined }
  ): void {
    this._azExtOutputChannel.appendLog(value, options);
  }
  append(value: string): void {
    this._azExtOutputChannel.append(value);
  }
  appendLine(value: string): void {
    const updatedValue = this.removePropertiesWithPossibleUserInfo(value);
    this._azExtOutputChannel.appendLine(updatedValue);
  }
  replace(value: string): void {
    this._azExtOutputChannel.replace(value);
  }
  clear(): void {
    this._azExtOutputChannel.clear();
  }
  show(preserveFocus?: boolean): void;
  show(column?: ViewColumn, preserveFocus?: boolean): void;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  show(column?: any, preserveFocus?: any): void {
    this._azExtOutputChannel.show(column, preserveFocus);
  }
  hide(): void {
    this._azExtOutputChannel.hide();
  }
  dispose(): void {
    this._azExtOutputChannel.dispose();
  }

  private removePropertiesWithPossibleUserInfo(value: string) {
    const deployParamsPattern = new RegExp(
      '.*"token":\\s*"(?<token>.*)",\\s.*"expiresOnTimestamp":\\s*"(?<expiresOnTimestamp>.*)".*'
    );
    const matches = deployParamsPattern.exec(value);

    if (matches != null) {
      const groups = matches.groups;

      if (groups != null) {
        const token = groups["token"];
        const expiresOnTimestamp = groups["expiresOnTimestamp"];

        let updatedValue = value.replace(token, "<REDACTED: token>");
        updatedValue = updatedValue.replace(
          expiresOnTimestamp,
          "<REDACTED: expiresOnTimestamp>"
        );

        return updatedValue;
      }
    }

    return value;
  }
}
