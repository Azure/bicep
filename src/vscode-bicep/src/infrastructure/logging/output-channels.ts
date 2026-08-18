// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { IAzExtLogOutputChannel, IAzExtOutputChannel } from "@microsoft/vscode-azext-utils";
import { LogOutputChannel, ViewColumn, window } from "vscode";
import { getBicepConfiguration } from "../configuration";
import { Disposable } from "../lifecycle";

type OutputSanitizer = (value: string) => string;

export function createAzExtOutputChannel(
  name: string,
  extensionConfigurationPrefix: string,
  sanitize: OutputSanitizer = (value) => value,
): IAzExtLogOutputChannel {
  return new AzExtOutputChannel(name, extensionConfigurationPrefix, sanitize);
}

class AzExtOutputChannel implements IAzExtLogOutputChannel {
  public readonly name: string;
  public readonly extensionConfigurationPrefix: string;
  private _outputChannel: LogOutputChannel;

  constructor(
    name: string,
    extensionConfigurationPrefix: string,
    private readonly sanitize: OutputSanitizer,
  ) {
    this.name = name;
    this.extensionConfigurationPrefix = extensionConfigurationPrefix;
    this._outputChannel = window.createOutputChannel(this.name, { log: true });
  }

  public get logLevel() {
    return this._outputChannel.logLevel;
  }

  public get onDidChangeLogLevel() {
    return this._outputChannel.onDidChangeLogLevel;
  }

  public replace(value: string): void {
    this._outputChannel.replace(value);
  }

  public append(value: string): void {
    this._outputChannel.append(value);
  }

  public appendLine(value: string): void {
    this._outputChannel.appendLine(this.sanitize(value));
  }

  public trace(message: string, ...args: unknown[]): void {
    this._outputChannel.trace(this.sanitize(message), ...args);
  }

  public debug(message: string, ...args: unknown[]): void {
    this._outputChannel.debug(this.sanitize(message), ...args);
  }

  public info(message: string, ...args: unknown[]): void {
    this._outputChannel.info(this.sanitize(message), ...args);
  }

  public warn(message: string, ...args: unknown[]): void {
    this._outputChannel.warn(this.sanitize(message), ...args);
  }

  public error(error: string | Error, ...args: unknown[]): void {
    this._outputChannel.error(typeof error === "string" ? this.sanitize(error) : error, ...args);
  }

  public appendLog(value: string, options?: { resourceName?: string; date?: Date }): void {
    const enableOutputTimestampsSetting = "enableOutputTimestamps";
    const result: boolean | undefined = getBicepConfiguration().get<boolean>(enableOutputTimestampsSetting);

    if (!result) {
      this.appendLine(value);
    } else {
      options ||= {};
      const date: Date = options.date || new Date();
      this.appendLine(
        `${date.toLocaleTimeString()}${options.resourceName ? " ".concat(options.resourceName) : ""}: ${value}`,
      );
    }
  }

  public clear(): void {
    this._outputChannel.clear();
  }

  public show(preserveFocus?: boolean | undefined): void;
  public show(column?: ViewColumn | undefined, preserveFocus?: boolean | undefined): void;
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

export class OutputChannelManager extends Disposable {
  private _azExtOutputChannel: IAzExtOutputChannel;

  constructor(name: string, extensionConfigurationPrefix: string, sanitize?: OutputSanitizer) {
    super();
    this._azExtOutputChannel = this.register(createAzExtOutputChannel(name, extensionConfigurationPrefix, sanitize));
  }

  appendToOutputChannel(text: string, noFocus = false): void {
    if (!noFocus) {
      this._azExtOutputChannel.show();
    }

    this._azExtOutputChannel.appendLog(text);
  }
}
