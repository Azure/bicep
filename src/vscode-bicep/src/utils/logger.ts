// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import * as winston from "winston";
import * as Transport from "winston-transport";
import { MESSAGE } from "triple-beam";

export interface Logger {
  debug(message: string): void;
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  error(error: Error): void;
}

export type LogLevel = keyof Logger;

let logger: Logger | undefined;

export class WinstonLogger implements Logger, vscode.Disposable {
  private readonly logger: winston.Logger;
  private disposed = false;

  constructor(outputChannel: vscode.OutputChannel, logLevel: LogLevel) {
    this.logger = winston.createLogger({
      level: logLevel,
      format: winston.format.combine(
        winston.format.timestamp(),
        winston.format.errors({ stack: true }),
        winston.format.printf((entry) =>
          entry.stack
            ? `${entry.timestamp} ${entry.level}: ${entry.message} - ${entry.stack}`
            : `${entry.timestamp} ${entry.level}: ${entry.message}`
        )
      ),
      transports: [new outputChannelTransport(outputChannel)],
    });
  }

  dispose(): void {
    if (!this.disposed) {
      this.logger.clear();
      this.logger.close();
      this.disposed = true;
    }
  }

  debug(message: string): void {
    this.logger.log("debug", message);
  }

  info(message: string): void {
    this.logger.log("info", message);
  }

  warn(message: string): void {
    this.logger.log("warn", message);
  }

  error(message: string | Error): void {
    this.logger.log("error", message);
  }
}

class outputChannelTransport extends Transport {
  constructor(private readonly outputChannel: vscode.OutputChannel) {
    super();
  }

  public log(entry: { [MESSAGE]: string }, next: () => void) {
    setImmediate(() => this.outputChannel.appendLine(entry[MESSAGE]));
    next();
  }
}

export function createLogger(context: vscode.ExtensionContext, outputChannel: vscode.OutputChannel): void {
  // TODO:
  // - make log level configurable
  // - Default log level should be info
  const winstonLogger = new WinstonLogger(outputChannel, "debug");

  logger = winstonLogger;
  logger.info("Current log level: debug.");

  context.subscriptions.push(winstonLogger);
}

export function getLogger(): Logger {
  if (!logger) {
    throw new Error(
      "Logger is undefined. Make sure to call createLogger() first."
    );
  }

  return logger;
}
