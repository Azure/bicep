/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AzureAccountTreeItemBase } from '@microsoft/vscode-azext-azureutils';
import { ISubscriptionContext } from '@microsoft/vscode-azext-utils';
import { ResourceGroupTreeItem } from './ResourceGroupTreeItem';

export class AzResourceGroupTreeItem extends AzureAccountTreeItemBase {
  public constructor(testAccount?: {}) {
    super(undefined, testAccount);
  }

  public createSubscriptionTreeItem(root: ISubscriptionContext): ResourceGroupTreeItem {
    return new ResourceGroupTreeItem(this, root);
  }
}
