// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { SubscriptionTreeItemBase } from "@microsoft/vscode-azext-azureutils";
import { AzExtTreeItem, IActionContext } from "@microsoft/vscode-azext-utils";
import { createSubscriptionClient } from "../utils/azureClients";
import { SubscriptionClient } from "@azure/arm-resources-subscriptions";
import { localize } from "../utils/localize";
import { GenericTreeItem } from "./GenericTreeItem";

export class LocationTreeItem extends SubscriptionTreeItemBase {
  public readonly childTypeLabel: string = localize("location", "Location");

  private _nextLink: string | undefined;

  public hasMoreChildrenImpl(): boolean {
    return !!this._nextLink;
  }

  public async loadMoreChildrenImpl(
    clearCache: boolean,
    context: IActionContext
  ): Promise<AzExtTreeItem[]> {
    if (clearCache) {
      this._nextLink = undefined;
    }
    const client: SubscriptionClient = await createSubscriptionClient([
      context,
      this,
    ]);

    const subscriptionId = this.subscription.subscriptionId;
    const locations = await client.subscriptions.listLocations(subscriptionId);

    const locationItems = await this.createTreeItemsWithErrorHandling(
      locations,
      "invalidLocation",
      (location) => new GenericTreeItem(this, location.id, location.name),
      (location) => location.name
    );

    return locationItems;
  }
}
