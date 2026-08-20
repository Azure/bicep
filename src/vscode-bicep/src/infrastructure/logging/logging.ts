// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { closeSync, openSync, writeSync } from "fs";
import * as path from "path";
import { fileURLToPath } from "url";
import { Disposable, ExtensionContext, LogOutputChannel } from "vscode";
import { parseError } from "../errors";

/**
 * This log file is written during E2E tests. It serves as a way to watch for events from the code
 * while running inside the tests, since simple in-memory sharing won't work.
 */
export const e2eLogName = "bicep-e2e.log";

export interface Logger extends Disposable {
  debug(message: string): void;
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  error(error: Error): void;
}

export type LogLevel = "debug" | "info" | "warn" | "error";
type LoggerOutputChannel = Pick<LogOutputChannel, "debug" | "info" | "warn" | "error">;
type LogFileSink = {
  log(level: LogLevel, message: string | Error): void;
  dispose(): void;
};

let logger: Logger | undefined;

export class BicepLogger implements Logger {
  private disposed = false;

  constructor(
    private readonly outputChannel: LoggerOutputChannel,
    private readonly logFileSink = createE2eLogFileSink(),
  ) {}

  dispose(): void {
    if (!this.disposed) {
      this.logFileSink?.dispose();
      this.disposed = true;
    }
  }

  debug(message: string): void {
    this.outputChannel.debug(message);
    this.logFileSink?.log("debug", message);
  }

  info(message: string): void {
    this.outputChannel.info(message);
    this.logFileSink?.log("info", message);
  }

  warn(message: string): void {
    this.outputChannel.warn(message);
    this.logFileSink?.log("warn", message);
  }

  error(message: string | Error): void {
    this.outputChannel.error(formatMessage(message));
    this.logFileSink?.log("error", message);
  }
}

class E2eLogFileSink implements LogFileSink {
  private readonly fileDescriptor: number;
  private disposed = false;

  constructor(filePath: string) {
    this.fileDescriptor = openSync(filePath, "w");
  }

  log(level: LogLevel, message: string | Error): void {
    if (this.disposed) {
      throw new Error("Cannot write to a disposed E2E log file.");
    }

    writeSync(this.fileDescriptor, `${new Date().toISOString()} ${level}: ${formatMessage(message)}\n`);
  }

  dispose(): void {
    if (!this.disposed) {
      closeSync(this.fileDescriptor);
      this.disposed = true;
    }
  }
}

function formatMessage(message: string | Error): string {
  return message instanceof Error
    ? message.stack
      ? `${message.message} - ${message.stack}`
      : message.message
    : message;
}

function createE2eLogFileSink(): LogFileSink | undefined {
  if (process.env.TEST_MODE !== "e2e") {
    return undefined;
  }

  const extensionPath = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
  return new E2eLogFileSink(path.join(extensionPath, e2eLogName));
}

export function createLogger(
  context: Pick<ExtensionContext, "subscriptions">,
  outputChannel: LoggerOutputChannel,
  logFileSink?: LogFileSink,
): Logger {
  const bicepLogger = new BicepLogger(outputChannel, logFileSink);

  logger = bicepLogger;

  context.subscriptions.push(bicepLogger);

  return logger;
}

export function getLogger(): Logger {
  if (!logger) {
    throw new Error("Logger is undefined. Make sure to call createLogger() first.");
  }

  return logger;
}

export function resetLogger(): void {
  logger = undefined;
}

export async function activateWithErrorHandling(activateCallback: () => Promise<void>): Promise<void> {
  const startTime = Date.now();

  try {
    await activateCallback();
  } catch (error) {
    getLogger().error(parseError(error).message);
    throw error;
  }

  const duration = (Date.now() - startTime) / 1000;
  getLogger().info(`Bicep extension activated in ${duration}s.`);
}
