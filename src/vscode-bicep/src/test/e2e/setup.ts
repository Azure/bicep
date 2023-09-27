// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

jest.setTimeout(20000);

// Workaround for https://github.com/microsoft/vscode-test/issues/37.
// eslint-disable-next-line @typescript-eslint/no-explicit-any
jest.mock("vscode", () => (global as any).vscode, { virtual: true });
