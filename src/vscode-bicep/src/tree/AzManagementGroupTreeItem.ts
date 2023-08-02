// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzureAccountTreeItemBase } from "@microsoft/vscode-azext-azureutils";
import { ISubscriptionContext } from "@microsoft/vscode-azext-utils";
import { ManagementGroupTreeItem } from "./ManagementGroupTreeItem";

// The root of treeview used in management group scope deployment. Represents an Azure account
export class AzManagementGroupTreeItem extends AzureAccountTreeItemBase {
  public createSubscriptionTreeItem(
    root: ISubscriptionContext,
  ): ManagementGroupTreeItem {
    return new ManagementGroupTreeItem(this, root);
  }
}
