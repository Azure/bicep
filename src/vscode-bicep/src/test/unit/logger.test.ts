// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
jest.mock("winston", () => ({
  createLogger: () => mockWinstonLogger,
  format: {
    combine: jest.fn(),
    timestamp: jest.fn(),
    errors: jest.fn(),
    printf: jest.fn(),
  },
}));

import * as vscode from "vscode";
import * as winston from "winston";

import * as loggerModule from "../../utils/logger";
import { expectDefined } from "../utils";

const mockWinstonLogger = ({
  clear: jest.fn(),
  close: jest.fn(),
  log: jest.fn(),
} as unknown) as winston.Logger;

const mockState: vscode.Memento = { get: jest.fn(), update: jest.fn() };
const mockContext: vscode.ExtensionContext = {
  subscriptions: [],
  workspaceState: mockState,
  globalState: mockState,
  asAbsolutePath: jest.fn(),
  extensionPath: "",
  storagePath: "",
  logPath: "",
};
const mockOutputChannel: vscode.OutputChannel = {
  name: "",
  append: jest.fn(),
  appendLine: jest.fn(),
  clear: jest.fn(),
  dispose: jest.fn(),
  hide: jest.fn(),
  show: jest.fn(),
};

const {
  createLogger,
  getLogger,
  resetLogger,
  WinstonLogger,
} = loggerModule as typeof loggerModule & {
  resetLogger: () => void;
};

describe("Logger initialization tests", () => {
  beforeEach(() => {
    jest.clearAllMocks();
    resetLogger();
  });

  test("createLogger() should add a new logger to disposibles subscription", () => {
    createLogger(mockContext, mockOutputChannel);

    expect(mockContext.subscriptions).toHaveLength(1);
  });

  test("getLogger() should throw if createLogger() is not called first", () => {
    expect(() => getLogger()).toThrow(
      "Logger is undefined. Make sure to call createLogger() first."
    );
  });

  test("getLogger() should return a logger if createLogger() is called first", () => {
    createLogger(mockContext, mockOutputChannel);

    expectDefined(getLogger);
  });
});

describe("WinstonLogger tests", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  test("calling dispose() multiple times should only dispose the logger once", () => {
    const logger = new WinstonLogger(mockOutputChannel, "info");

    logger.dispose();
    logger.dispose();
    logger.dispose();

    expect(mockWinstonLogger.clear).toHaveBeenCalledTimes(1);
    expect(mockWinstonLogger.close).toHaveBeenCalledTimes(1);
  });

  test.each(["debug", "info", "warn", "error"] as const)(
    "calling %s should log message at %s level",
    (level) => {
      const logger = new WinstonLogger(mockOutputChannel, "info");

      logger[level]("something");

      expect(mockWinstonLogger.log).toHaveBeenNthCalledWith(
        1,
        level,
        "something"
      );
    }
  );
});
