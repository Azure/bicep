// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { TreeManager } from "../tree/TreeManager";
import { DeploymentScope, DeploymentScopeType, IAzureUiManager } from "./types";

function getSubscriptionId(scope: DeploymentScope) {
  switch (scope.scopeType) {
    case "resourceGroup":
      return scope.subscriptionId;
    case "subscription":
      return scope.subscriptionId;
    case "managementGroup":
      return scope.associatedSubscriptionId;
    case "tenant":
      return scope.associatedSubscriptionId;
  }
}

export class AzureUiManager implements IAzureUiManager {
  public constructor(
    private readonly context: IActionContext,
    private readonly treeManager: TreeManager,
  ) {}

  public async getAccessToken(scope: DeploymentScope): Promise<AccessToken> {
    const subscriptionId = getSubscriptionId(scope);

    const scopeId = `/subscriptions/${subscriptionId}`;
    const treeItem = await this.treeManager.azLocationTree.findTreeItem(scopeId, this.context);

    if (!treeItem) {
      throw `Failed to authenticate with Azure for subscription ${subscriptionId}. Are you signed in to the Azure VSCode extension under the correct account?`;
    }

    return treeItem.subscription.credentials.getToken();
  }

  public async pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope> {
    switch (scopeType) {
      case "resourceGroup": {
        const treeItem = await this.treeManager.azResourceGroupTreeItem.showTreeItemPicker("", this.context);

        return {
          scopeType,
          portalUrl: treeItem.subscription.environment.portalUrl,
          tenantId: treeItem.subscription.tenantId,
          subscriptionId: treeItem.subscription.subscriptionId,
          resourceGroup: treeItem.label,
        };
      }
      case "subscription": {
        const treeItem = await this.treeManager.azLocationTree.showTreeItemPicker("", this.context);
        return {
          scopeType,
          portalUrl: treeItem.subscription.environment.portalUrl,
          tenantId: treeItem.subscription.tenantId,
          subscriptionId: treeItem.subscription.subscriptionId,
          location: treeItem.label,
        };
      }
      case "managementGroup": {
        const locationItem = await this.treeManager.azLocationTree.showTreeItemPicker("", this.context);
        const treeItem = await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker("", this.context);

        return {
          scopeType,
          portalUrl: locationItem.subscription.environment.portalUrl,
          tenantId: treeItem.subscription.tenantId,
          managementGroup: treeItem.label,
          associatedSubscriptionId: locationItem.subscription.subscriptionId,
          location: locationItem.label,
        };
      }
      case "tenant": {
        const locationItem = await this.treeManager.azLocationTree.showTreeItemPicker("", this.context);

        return {
          scopeType,
          portalUrl: locationItem.subscription.environment.portalUrl,
          tenantId: locationItem.subscription.tenantId,
          associatedSubscriptionId: locationItem.subscription.subscriptionId,
          location: locationItem.label,
        };
      }
    }
  }
}
