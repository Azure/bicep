// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Mock for the vscode API object. The export name must match the original module.
export const vscode = {
  postMessage: jest.fn(),
  setState: jest.fn(),
  getState: jest.fn(),
};
