// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

declare function acquireVsCodeApi(): {
  postMessage(message: unknown): void;
  setState(state: unknown): void;
  getState<T>(): T;
};

export const vscode = acquireVsCodeApi();
