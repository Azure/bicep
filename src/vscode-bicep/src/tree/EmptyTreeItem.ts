/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AzExtTreeItem, AzExtParentTreeItem } from 'vscode-azureextensionui';

export class EmptyTreeItem extends AzExtTreeItem {
  public readonly contextValue: string = '';
  constructor(parent: AzExtParentTreeItem) {
    super(parent);
  }

  public get id(): string {
    return '';
  }

  public get label(): string {
    return '';
  }
}
