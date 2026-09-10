// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { LogOutputChannel, ViewColumn, window } from "vscode";
import { getBicepConfiguration } from "../configuration";
import { Disposable } from "../lifecycle";

type OutputSanitizer = (value: string) => string;

export interface BicepLogOutputChannel extends LogOutputChannel {
  appendLog(value: string, options?: { resourceName?: string; date?: Date }): void;
}

export function createLogOutputChannel(
  name: string,
  sanitize: OutputSanitizer = (value) => value,
): BicepLogOutputChannel {
  return new BicepOutputChannel(name, sanitize);
}

class BicepOutputChannel implements BicepLogOutputChannel {
  public readonly name: string;
  private readonly outputChannel: LogOutputChannel;

  constructor(
    name: string,
    private readonly sanitize: OutputSanitizer,
  ) {
    this.name = name;
    this.outputChannel = window.createOutputChannel(this.name, { log: true });
  }

  public get logLevel() {
    return this.outputChannel.logLevel;
  }

  public get onDidChangeLogLevel() {
    return this.outputChannel.onDidChangeLogLevel;
  }

  public replace(value: string): void {
    this.outputChannel.replace(value);
  }

  public append(value: string): void {
    this.outputChannel.append(value);
  }

  public appendLine(value: string): void {
    this.outputChannel.appendLine(this.sanitize(value));
  }

  public trace(message: string, ...args: unknown[]): void {
    this.outputChannel.trace(this.sanitize(message), ...args);
  }

  public debug(message: string, ...args: unknown[]): void {
    this.outputChannel.debug(this.sanitize(message), ...args);
  }

  public info(message: string, ...args: unknown[]): void {
    this.outputChannel.info(this.sanitize(message), ...args);
  }

  public warn(message: string, ...args: unknown[]): void {
    this.outputChannel.warn(this.sanitize(message), ...args);
  }

  public error(error: string | Error, ...args: unknown[]): void {
    this.outputChannel.error(typeof error === "string" ? this.sanitize(error) : error, ...args);
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
    this.outputChannel.clear();
  }

  public show(preserveFocus?: boolean | undefined): void;
  public show(column?: ViewColumn | undefined, preserveFocus?: boolean | undefined): void;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public show(_column?: any, preserveFocus?: boolean | undefined): void {
    this.outputChannel.show(preserveFocus);
  }

  public hide(): void {
    this.outputChannel.hide();
  }

  public dispose(): void {
    this.outputChannel.dispose();
  }
}

export class OutputChannelManager extends Disposable {
  private readonly outputChannel: BicepLogOutputChannel;

  constructor(name: string, sanitize?: OutputSanitizer) {
    super();
    this.outputChannel = this.register(createLogOutputChannel(name, sanitize));
  }

  appendToOutputChannel(text: string, noFocus = false): void {
    if (!noFocus) {
      this.outputChannel.show();
    }

    this.outputChannel.appendLog(text);
  }
}
