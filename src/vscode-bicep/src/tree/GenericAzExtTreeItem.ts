// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  AzExtParentTreeItem,
  AzExtTreeItem,
} from "@microsoft/vscode-azext-utils";

export class GenericAzExtTreeItem extends AzExtTreeItem {
  private _id: string | undefined;
  private _label: string | undefined;

  constructor(
    parent: AzExtParentTreeItem,
    id: string | undefined,
    label: string | undefined,
  ) {
    super(parent);
    this._id = id;
    this._label = label;
  }

  public get id(): string | undefined {
    return this._id;
  }

  public get label(): string {
    const label = this._label;

    if (label) {
      return label;
    }

    return "";
  }

  public readonly contextValue: string = "";
}
