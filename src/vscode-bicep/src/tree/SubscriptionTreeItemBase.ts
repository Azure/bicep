// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//asdfg new (copied)

import { AzExtParentTreeItem, ISubscriptionContext } from '@microsoft/vscode-azext-utils';
//asdfg import { getIconPath } from './IconPath';

export abstract class SubscriptionTreeItemBase extends AzExtParentTreeItem {
    public static readonly contextValue: string = 'azureextensionui.azureSubscription';
    public readonly contextValue: string = SubscriptionTreeItemBase.contextValue;
    public readonly label: string;

    public constructor(parent: AzExtParentTreeItem | undefined, subscription: ISubscriptionContext) {
        super(parent);
        // eslint-disable-next-line @typescript-eslint/ban-ts-comment
        // @ts-ignore
        this._subscription = subscription;
        this.label = subscription.subscriptionDisplayName;
        this.id = subscription.subscriptionPath;
       //asdfg  this.iconPath = getIconPath('azureSubscription');
    }
}
