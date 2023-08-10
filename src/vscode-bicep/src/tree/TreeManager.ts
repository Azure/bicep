// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";

import { Disposable } from "../utils/disposable";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { AzLocationTreeItem } from "./AzLocationTreeItem";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";

export class TreeManager extends Disposable {
  constructor(private readonly outputChannelManager: OutputChannelManager) {
    super();
  }

  get azLocationTree(): AzExtTreeDataProvider {
    const azLocationTreeItem: AzLocationTreeItem = this.register(
      new AzLocationTreeItem(),
    );
    return new AzExtTreeDataProvider(azLocationTreeItem, "");
  }

  get azManagementGroupTreeItem(): AzExtTreeDataProvider {
    const azManagementGroupTreeItem: AzManagementGroupTreeItem = this.register(
      new AzManagementGroupTreeItem(),
    );
    return new AzExtTreeDataProvider(azManagementGroupTreeItem, "");
  }

  get azResourceGroupTreeItem(): AzExtTreeDataProvider {
    const azResourceGroupTreeItem: AzResourceGroupTreeItem = this.register(
      new AzResourceGroupTreeItem(this.outputChannelManager),
    );
    return new AzExtTreeDataProvider(azResourceGroupTreeItem, "");
  }
}
