// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { BicepLogger, createLogger, getLogger, resetLogger } from "../logging";

function createLoggerTracker() {
  const logged: [string, unknown][] = [];
  let disposeCount = 0;
  const outputChannel = {
    debug: (message: unknown) => logged.push(["debug", message]),
    info: (message: unknown) => logged.push(["info", message]),
    warn: (message: unknown) => logged.push(["warn", message]),
    error: (message: unknown) => logged.push(["error", message]),
  };
  const logFileSink = {
    log: (level: string, message: unknown) => {
      logged.push([level, message]);
    },
    dispose: () => {
      disposeCount++;
    },
  };

  return {
    get disposeCount() {
      return disposeCount;
    },
    logged,
    logFileSink,
    outputChannel,
  };
}

describe("createLogger", () => {
  it("adds the logger to extension subscriptions", () => {
    const context = { subscriptions: [] };
    const tracker = createLoggerTracker();

    createLogger(context, tracker.outputChannel, tracker.logFileSink);

    expect(context.subscriptions).toHaveLength(1);
  });
});

describe("getLogger", () => {
  it("throws before a logger is created", () => {
    resetLogger();
    expect(() => getLogger()).toThrow("Logger is undefined. Make sure to call createLogger() first.");
  });

  it("returns the created logger", () => {
    resetLogger();
    const context = { subscriptions: [] };
    const tracker = createLoggerTracker();
    const created = createLogger(context, tracker.outputChannel, tracker.logFileSink);

    expect(getLogger()).toBe(created);
  });
});

describe("BicepLogger", () => {
  it("disposes the underlying logger only once", () => {
    const tracker = createLoggerTracker();
    const logger = new BicepLogger(tracker.outputChannel, tracker.logFileSink);

    logger.dispose();
    logger.dispose();
    logger.dispose();

    expect(tracker.disposeCount).toBe(1);
  });

  it.each(["debug", "info", "warn", "error"] as const)("logs messages at the %s level to both sinks", (level) => {
    const tracker = createLoggerTracker();
    const logger = new BicepLogger(tracker.outputChannel, tracker.logFileSink);

    logger[level]("something");

    expect(tracker.logged).toEqual([
      [level, "something"],
      [level, "something"],
    ]);
  });

  it("formats errors for the output channel and forwards them to the file sink", () => {
    const tracker = createLoggerTracker();
    const logger = new BicepLogger(tracker.outputChannel, tracker.logFileSink);
    const error = new Error("something went wrong");

    logger.error(error);

    expect(tracker.logged).toEqual([
      ["error", `${error.message} - ${error.stack}`],
      ["error", error],
    ]);
  });
});
