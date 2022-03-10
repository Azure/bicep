// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";

import { AzLocationTreeItem } from "./AzLocationTreeItem";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";

export class TreeManager {
  private _azLocationTree: AzExtTreeDataProvider;
  private _azManagementGroupTreeItem: AzExtTreeDataProvider;
  private _azResourceGroupTreeItem: AzExtTreeDataProvider;

  constructor() {
    const azLocationTreeItem: AzLocationTreeItem = new AzLocationTreeItem();
    this._azLocationTree = new AzExtTreeDataProvider(azLocationTreeItem, "");

    const azManagementGroupTreeItem: AzManagementGroupTreeItem =
      new AzManagementGroupTreeItem();
    this._azManagementGroupTreeItem = new AzExtTreeDataProvider(
      azManagementGroupTreeItem,
      ""
    );

    const azResourceGroupTreeItem: AzResourceGroupTreeItem =
      new AzResourceGroupTreeItem();
    this._azResourceGroupTreeItem = new AzExtTreeDataProvider(
      azResourceGroupTreeItem,
      ""
    );
  }

  get azLocationTree(): AzExtTreeDataProvider {
    return this._azLocationTree;
  }

  get azManagementGroupTreeItem(): AzExtTreeDataProvider {
    return this._azManagementGroupTreeItem;
  }

  get azResourceGroupTreeItem(): AzExtTreeDataProvider {
    return this._azResourceGroupTreeItem;
  }
}
