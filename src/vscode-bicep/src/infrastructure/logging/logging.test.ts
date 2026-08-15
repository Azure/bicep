// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { createLogger, getLogger, resetLogger, WinstonLogger } from "./logging";

const outputChannel = { appendLine: () => undefined };

function createWinstonLoggerTracker() {
  const logged: [string, unknown][] = [];
  let clearCount = 0;
  let closeCount = 0;
  const logger = {
    clear: () => {
      clearCount++;
    },
    close: () => {
      closeCount++;
    },
    log: (level: string, message: unknown) => {
      logged.push([level, message]);
    },
  };

  return {
    create: () => logger,
    get clearCount() {
      return clearCount;
    },
    get closeCount() {
      return closeCount;
    },
    logged,
  };
}

describe("createLogger", () => {
  it("adds the logger to extension subscriptions", () => {
    const context = { subscriptions: [] };
    const tracker = createWinstonLoggerTracker();

    createLogger(context, outputChannel, tracker.create);

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
    const tracker = createWinstonLoggerTracker();
    const created = createLogger(context, outputChannel, tracker.create);

    expect(getLogger()).toBe(created);
  });
});

describe("WinstonLogger", () => {
  it("disposes the underlying logger only once", () => {
    const tracker = createWinstonLoggerTracker();
    const logger = new WinstonLogger(outputChannel, "info", tracker.create);

    logger.dispose();
    logger.dispose();
    logger.dispose();

    expect(tracker.clearCount).toBe(1);
    expect(tracker.closeCount).toBe(1);
  });

  it.each(["debug", "info", "warn", "error"] as const)("logs messages at the %s level", (level) => {
    const tracker = createWinstonLoggerTracker();
    const logger = new WinstonLogger(outputChannel, "info", tracker.create);

    logger[level]("something");

    expect(tracker.logged).toEqual([[level, "something"]]);
  });
});
