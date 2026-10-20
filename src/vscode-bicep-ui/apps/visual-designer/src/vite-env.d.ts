// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/// <reference types="vite/client" />

declare module "@vscode-elements/webview-playground";

/** The playground's snapshots of VS Code's built-in color themes, as CSS variable name/value pairs. */
declare module "@vscode-elements/webview-playground/dist/themes/*.js" {
  export const theme: [string, string][];
}
