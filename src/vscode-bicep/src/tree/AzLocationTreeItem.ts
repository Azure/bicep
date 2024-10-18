// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg import { AzureAccountTreeItemBase } from "@microsoft/vscode-azext-azureutils";
import { ISubscriptionContext } from "@microsoft/vscode-azext-utils"; //asdfg
import { LocationTreeItem } from "./LocationTreeItem";
import { AzureAccountTreeItemBase } from "./AzureAccountTreeItemBase";

// The root of treeview used in subscription scope deployment. Represents an Azure account
export class AzLocationTreeItem extends AzureAccountTreeItemBase {
  public createSubscriptionTreeItem(root: ISubscriptionContext): LocationTreeItem {
    return new LocationTreeItem(this, root);
  }
}
