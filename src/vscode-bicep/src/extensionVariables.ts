// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";

/**
 * Namespace for common variables used throughout the extension. They must be initialized in the activate() method of extension.ts
 */
class ExtensionVariables {
  public azLocationTree!: AzExtTreeDataProvider;
  public azLoginTreeItem!: AzExtTreeDataProvider;
  public azManagementGroupTreeItem!: AzExtTreeDataProvider;
  public azResourceGroupTreeItem!: AzExtTreeDataProvider;
}

const ext = new ExtensionVariables();
export { ext };
