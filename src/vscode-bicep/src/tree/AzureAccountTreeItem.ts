/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AzureAccountTreeItemBase, ISubscriptionContext } from 'vscode-azureextensionui';
import { SubscriptionTreeItem } from './SubscriptionTreeItem';

export class AzureAccountTreeItem extends AzureAccountTreeItemBase {
  public constructor(testAccount?: {}) {
    super(undefined, testAccount);
  }

  public createSubscriptionTreeItem(root: ISubscriptionContext): SubscriptionTreeItem {
    return new SubscriptionTreeItem(this, root);
  }
}
