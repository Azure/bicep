// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  AzExtTreeDataProvider,
  IAzExtOutputChannel,
} from "@microsoft/vscode-azext-utils";

class ExtensionVariables {
  public azLocationTree!: AzExtTreeDataProvider;
  public azManagementGroupTreeItem!: AzExtTreeDataProvider;
  public azResourceGroupTreeItem!: AzExtTreeDataProvider;
  public bicepOperationsOutputChannel!: IAzExtOutputChannel;
}

const ext = new ExtensionVariables();
export { ext };
