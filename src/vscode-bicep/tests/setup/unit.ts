// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import mockRequire from "mock-require";

export class CancellationError extends Error {}

export const ConfigurationTarget = { Global: 1 };
export const ProgressLocation = { Notification: 15 };
export class ThemeColor {}
export class ThemeIcon {}
export const window = {};
export const l10n = { t: (message: string): string => message };

const vscode = {
  CancellationError,
  ConfigurationTarget,
  ProgressLocation,
  ThemeColor,
  ThemeIcon,
  window,
  l10n,
};

mockRequire("vscode", vscode);

export default vscode;
