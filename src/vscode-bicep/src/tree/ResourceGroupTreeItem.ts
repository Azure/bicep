// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import {
  ResourceGroup,
} from "@azure/arm-resources";
import {
  AzExtParentTreeItem,
  AzExtTreeItem,
} from "@microsoft/vscode-azext-utils";

export class ResourceGroupTreeItem extends AzExtTreeItem {
  public data: ResourceGroup;

  constructor(parent: AzExtParentTreeItem, rg: ResourceGroup) {
    super(parent);
    this.data = rg;
  }

  public get resourceGroup(): ResourceGroup {
    return this.data;
  }

  public get id(): string | undefined {
    return this.data.id;
  }

  public get label(): string  {
    const name = this.data.name;

    if (name) {
      return name;
    }

    return "";
  }

  public readonly contextValue: string = "";
}
