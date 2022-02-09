// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzExtTreeItem, AzExtParentTreeItem } from "vscode-azureextensionui";

export class EmptyTreeItem extends AzExtTreeItem {
  public readonly contextValue: string = "";
  constructor(parent: AzExtParentTreeItem) {
    super(parent);
  }

  public get id(): string {
    return "";
  }

  public get label(): string {
    return "";
  }
}
