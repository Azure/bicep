// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzureAccountTreeItemBase } from "@microsoft/vscode-azext-azureutils";
import { ISubscriptionContext } from "@microsoft/vscode-azext-utils";

import { ResourceGroupTreeItem } from "./ResourceGroupTreeItem";

// The root of treeview used in resource group scope deployment, represents an Azure account
export class AzResourceGroupTreeItem extends AzureAccountTreeItemBase {
  public constructor(testAccount?: {}) {
    super(undefined, testAccount);
  }

  public createSubscriptionTreeItem(
    root: ISubscriptionContext
  ): ResourceGroupTreeItem {
    return new ResourceGroupTreeItem(this, root);
  }
}
