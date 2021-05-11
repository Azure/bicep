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
  transports: {
    File: jest.fn(),
  },
}));

import * as vscode from "vscode";
import * as winston from "winston";

import * as loggerModule from "../../utils/logger";
import { expectDefined } from "../utils/assert";

const mockWinstonLogger = {
  clear: jest.fn(),
  close: jest.fn(),
  log: jest.fn(),
} as unknown as winston.Logger;

const mockWorkspaceState: vscode.Memento = {
  get: jest.fn(),
  update: jest.fn(),
};
const mockGlobalstate = { ...mockWorkspaceState, setKeysForSync: jest.fn() };
const mockContext = {
  subscriptions: [],
  workspaceState: mockWorkspaceState,
  globalState: mockGlobalstate,
  asAbsolutePath: jest.fn(),
  extensionPath: "",
  storagePath: "",
  logPath: "",
} as unknown as vscode.ExtensionContext;

const mockOutputChannel: vscode.OutputChannel = {
  name: "",
  append: jest.fn(),
  appendLine: jest.fn(),
  clear: jest.fn(),
  dispose: jest.fn(),
  hide: jest.fn(),
  show: jest.fn(),
};

const { createLogger, getLogger, resetLogger, WinstonLogger } =
  loggerModule as typeof loggerModule & {
    resetLogger: () => void;
  };

describe("createLogger()", () => {
  it("should add a new logger to disposibles subscription", () => {
    createLogger(mockContext, mockOutputChannel);

    expect(mockContext.subscriptions).toHaveLength(1);
  });
});

describe("getLogger()", () => {
  it("should throw if createLogger() is not called first", () => {
    resetLogger();
    expect(() => getLogger()).toThrow(
      "Logger is undefined. Make sure to call createLogger() first."
    );
  });

  it("should return a logger if createLogger() is called first", () => {
    resetLogger();
    createLogger(mockContext, mockOutputChannel);

    expectDefined(getLogger);
  });
});

// eslint-disable-next-line jest/lowercase-name
describe("WinstonLogger", () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("should only be disposed once even when dispose() is called multiple times", () => {
    const logger = new WinstonLogger(mockOutputChannel, "info");

    logger.dispose();
    logger.dispose();
    logger.dispose();

    expect(mockWinstonLogger.clear).toHaveBeenCalledTimes(1);
    expect(mockWinstonLogger.close).toHaveBeenCalledTimes(1);
  });

  it.each(["debug", "info", "warn", "error"] as const)(
    "should should log message at %s level",
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
