// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IAzExtOutputChannel } from "@microsoft/vscode-azext-utils";
import { OutputChannel, ViewColumn, window } from "vscode";
import { getBicepConfiguration } from "../language/getBicepConfiguration";
import { removePropertiesWithPossibleUserInfoInDeployParams } from "./removePropertiesWithPossibleUserInfo";

// https://github.com/microsoft/vscode-azuretools/blob/main/utils/src/AzExtOutputChannel.ts
// with support to remove properties with possible user info before appendLine(..) is invoked on output channel.
// TODO: revisit this when https://github.com/Azure/azure-sdk-for-net/issues/27263 is resolved.
export function createAzExtOutputChannel(
  name: string,
  extensionConfigurationPrefix: string,
): IAzExtOutputChannel {
  return new AzExtOutputChannel(name, extensionConfigurationPrefix);
}

class AzExtOutputChannel implements IAzExtOutputChannel {
  public readonly name: string;
  public readonly extensionConfigurationPrefix: string;
  private _outputChannel: OutputChannel;

  constructor(name: string, extensionConfigurationPrefix: string) {
    this.name = name;
    this.extensionConfigurationPrefix = extensionConfigurationPrefix;
    this._outputChannel = window.createOutputChannel(this.name);
  }

  public replace(value: string): void {
    this._outputChannel.replace(value);
  }

  public append(value: string): void {
    this._outputChannel.append(value);
  }

  public appendLine(value: string): void {
    const updatedValue =
      removePropertiesWithPossibleUserInfoInDeployParams(value);
    this._outputChannel.appendLine(updatedValue);
  }

  public appendLog(
    value: string,
    options?: { resourceName?: string; date?: Date },
  ): void {
    const enableOutputTimestampsSetting = "enableOutputTimestamps";
    const result: boolean | undefined = getBicepConfiguration().get<boolean>(
      enableOutputTimestampsSetting,
    );

    if (!result) {
      this.appendLine(value);
    } else {
      options ||= {};
      const date: Date = options.date || new Date();
      this.appendLine(
        `${date.toLocaleTimeString()}${
          options.resourceName ? " ".concat(options.resourceName) : ""
        }: ${value}`,
      );
    }
  }

  public clear(): void {
    this._outputChannel.clear();
  }

  public show(preserveFocus?: boolean | undefined): void;
  public show(
    column?: ViewColumn | undefined,
    preserveFocus?: boolean | undefined,
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
