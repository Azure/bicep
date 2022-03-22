// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";

import { Disposable } from "../utils/disposable";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { AzLocationTreeItem } from "./AzLocationTreeItem";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";

export class TreeManager extends Disposable {
  private _azLocationTree: AzExtTreeDataProvider;
  private _azManagementGroupTreeItem: AzExtTreeDataProvider;
  private _azResourceGroupTreeItem: AzExtTreeDataProvider;

  constructor(private readonly outputChannelManager: OutputChannelManager) {
    super();
    const azLocationTreeItem: AzLocationTreeItem = this.register(
      new AzLocationTreeItem()
    );
    this._azLocationTree = new AzExtTreeDataProvider(azLocationTreeItem, "");

    const azManagementGroupTreeItem: AzManagementGroupTreeItem =
      new AzManagementGroupTreeItem();
    this._azManagementGroupTreeItem = new AzExtTreeDataProvider(
      azManagementGroupTreeItem,
      ""
    );

    const azResourceGroupTreeItem: AzResourceGroupTreeItem = this.register(
      new AzResourceGroupTreeItem(this.outputChannelManager)
    );
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
