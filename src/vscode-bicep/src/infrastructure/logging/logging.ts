// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { callWithTelemetryAndErrorHandling, IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import * as path from "path";
import vscode from "vscode";
import * as winston from "winston";
import Transport from "winston-transport";

/**
 * This logfile is written during to E2E tests. It serves as a way to watch for events from the code
 * while running inside the tests, since simple in-memory sharing won't work.
 */
export const e2eLogName = "bicep-e2e.log";

export interface Logger extends vscode.Disposable {
  debug(message: string): void;
  info(message: string): void;
  warn(message: string): void;
  error(message: string): void;
  error(error: Error): void;
}

export type LogLevel = keyof Logger;
type WinstonLoggerSink = {
  clear(): unknown;
  close(): unknown;
  log(level: string, message: unknown): unknown;
};
type WinstonLoggerFactory = (options: winston.LoggerOptions) => WinstonLoggerSink;

let logger: Logger | undefined;

export class WinstonLogger implements Logger {
  private readonly logger: WinstonLoggerSink;
  private disposed = false;

  constructor(
    outputChannel: Pick<vscode.OutputChannel, "appendLine">,
    logLevel: LogLevel,
    createWinstonLogger: WinstonLoggerFactory = winston.createLogger,
  ) {
    this.logger = createWinstonLogger({
      level: logLevel,
      format: winston.format.combine(
        winston.format.timestamp(),
        winston.format.errors({ stack: true }),
        winston.format.printf((entry) =>
          entry.stack
            ? `${entry.timestamp} ${entry.level}: ${entry.message} - ${entry.stack}`
            : `${entry.timestamp} ${entry.level}: ${entry.message}`,
        ),
      ),
      transports: [
        new outputChannelTransport(outputChannel),
        ...(process.env.TEST_MODE === "e2e"
          ? [
              new winston.transports.File({
                dirname: path.resolve(__dirname, ".."),
                filename: e2eLogName,
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
  constructor(private readonly outputChannel: Pick<vscode.OutputChannel, "appendLine">) {
    super();
  }

  public log(entry: winston.Logform.TransformableInfo, next: () => void) {
    const message = entry[Symbol.for("message")];
    if (typeof message !== "string") {
      throw new Error("Expected a formatted Winston log entry.");
    }

    setImmediate(() => this.outputChannel.appendLine(message));
    next();
  }
}

export function createLogger(
  context: Pick<vscode.ExtensionContext, "subscriptions">,
  outputChannel: Pick<vscode.OutputChannel, "appendLine">,
  createWinstonLogger?: WinstonLoggerFactory,
): Logger {
  // TODO:
  // - make log level configurable
  // - Default log level should be info
  const winstonLogger = new WinstonLogger(outputChannel, "debug", createWinstonLogger);

  logger = winstonLogger;
  logger.info("Current log level: debug.");

  context.subscriptions.push(winstonLogger);

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

export async function activateWithTelemetryAndErrorHandling(
  activateCallback: (actionContext: IActionContext) => Promise<void>,
): Promise<void> {
  await callWithTelemetryAndErrorHandling("bicep.activate", async (actionContext: IActionContext) => {
    const startTime = Date.now();
    actionContext.telemetry.properties.isActivationEvent = "true";

    try {
      await activateCallback(actionContext);
    } catch (e) {
      getLogger().error(parseError(e).message);
      throw e;
    }

    actionContext.telemetry.measurements.extensionLoad = (Date.now() - startTime) / 1000;
  });
}

// Creates a possible telemetry event scope.  But the event is only sent if there is a cancel or an error
export async function callWithTelemetryAndErrorHandlingOnlyOnErrors<T>(
  callbackId: string,
  callback: (context: IActionContext) => T | PromiseLike<T>,
): Promise<T | undefined> {
  return await callWithTelemetryAndErrorHandling<T | undefined>(callbackId, async (context): Promise<T | undefined> => {
    context.telemetry.suppressIfSuccessful = true;

    return await callback(context);
  });
}

export async function raiseErrorWithoutTelemetry(callbackId: string, error: unknown) {
  await callWithTelemetryAndErrorHandling<void>(callbackId, (context) => {
    context.telemetry.suppressAll = true;

    return new Promise((_, reject) => reject(error));
  });
}
