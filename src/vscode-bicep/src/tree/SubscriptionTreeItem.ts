/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AzExtTreeItem, SubscriptionTreeItemBase } from 'vscode-azureextensionui';

import { EmptyTreeItem } from './EmptyTreeItem';

export class SubscriptionTreeItem extends SubscriptionTreeItemBase {

  private _nextLink: string | undefined;

  public hasMoreChildrenImpl(): boolean {
    return !!this._nextLink;
  }

  public async loadMoreChildrenImpl(clearCache: boolean): Promise<AzExtTreeItem[]> {
    if (clearCache) {
      this._nextLink = undefined;
    }

    return [new EmptyTreeItem(this)];
  }

  public async createChildImpl(): Promise<AzExtTreeItem> {
    return new EmptyTreeItem(this)
  }
}
