// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export const vscode = (): {
  postMessage(message: unknown): void;
  setState(state: unknown): void;
  getState<T>(): T;
} => ({
  postMessage: jest.fn(),
  setState: jest.fn(),
  getState: jest.fn(),
});
