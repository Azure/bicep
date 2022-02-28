// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzExtTreeDataProvider } from '@microsoft/vscode-azext-utils';

/**
 * Namespace for common variables used throughout the extension. They must be initialized in the activate() method of extension.ts
 */
export namespace ext {
  export let azLoginTreeItem: AzExtTreeDataProvider;
  export let azResourceGroupTreeItem: AzExtTreeDataProvider;
  export let azLocationTree: AzExtTreeDataProvider;
}
