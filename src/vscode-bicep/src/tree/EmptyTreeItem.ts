// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzExtTreeItem } from "@microsoft/vscode-azext-utils";

export class EmptyTreeItem extends AzExtTreeItem {
  public readonly contextValue: string = "";

  public get id(): string {
    return "";
  }

  public get label(): string {
    return "";
  }
}
