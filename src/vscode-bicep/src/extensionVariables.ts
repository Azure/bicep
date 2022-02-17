// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzExtTreeDataProvider, IAzExtOutputChannel } from '@microsoft/vscode-azext-utils';
import vscode from "vscode";

/**
 * Namespace for common variables used throughout the extension. They must be initialized in the activate() method of extension.ts
 */
export namespace ext {
  //export let context: vscode.ExtensionContext;
  //export let outputChannel: IAzExtOutputChannel;
  //export let ignoreBundle: boolean | undefined;

  export let tree: AzExtTreeDataProvider;
  export let azTree: AzExtTreeDataProvider;
}
