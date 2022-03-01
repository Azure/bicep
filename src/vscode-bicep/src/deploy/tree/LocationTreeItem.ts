// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { SubscriptionClient } from "@azure/arm-resources-subscriptions";
import { SubscriptionTreeItemBase } from "@microsoft/vscode-azext-azureutils";
import { AzExtTreeItem, IActionContext } from "@microsoft/vscode-azext-utils";

import { localize } from "../../utils/localize";
import { createSubscriptionClient } from "../azureClients";
import { GenericAzExtTreeItem } from "./GenericAzExtTreeItem";

// Represents an Azure subscription. Used to display locations related to the subscription
export class LocationTreeItem extends SubscriptionTreeItemBase {
  public readonly childTypeLabel: string = localize("location", "Location");

  private _nextLink: string | undefined;

  public hasMoreChildrenImpl(): boolean {
    return !!this._nextLink;
  }

  // Loads locations
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
      (location) => new GenericAzExtTreeItem(this, location.id, location.name),
      (location) => location.name
    );

    return locationItems;
  }
}
