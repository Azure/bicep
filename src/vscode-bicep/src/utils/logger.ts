// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import * as winston from "winston";
import Transport from "winston-transport";
import * as path from "path";
import { MESSAGE } from "triple-beam";

export interface Logger extends vscode.Disposable {
  debug(message: string): void;
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  error(error: Error): void;
}

export type LogLevel = keyof Logger;

let logger: Logger | undefined;

export class WinstonLogger implements Logger {
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
      transports: [
        new outputChannelTransport(outputChannel),
        ...(process.env.TEST_MODE === "e2e"
          ? [
              new winston.transports.File({
                dirname: path.resolve(__dirname, ".."),
                filename: "bicep.log",
                options: { flags: "w" },
              }),
            ]
          : []),
      ],
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

export function createLogger(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Logger {
  // TODO:
  // - make log level configurable
  // - Default log level should be info
  const winstonLogger = new WinstonLogger(outputChannel, "debug");

  logger = winstonLogger;
  logger.info("Current log level: debug.");

  context.subscriptions.push(winstonLogger);

  return logger;
}

export function getLogger(): Logger {
  if (!logger) {
    throw new Error(
      "Logger is undefined. Make sure to call createLogger() first."
    );
  }

  return logger;
}

export function resetLogger(): void {
  logger = undefined;
}
