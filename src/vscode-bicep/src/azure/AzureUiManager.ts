// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import { createSubscriptionContext, IActionContext } from "@microsoft/vscode-azext-utils";
import { TreeManager } from "../tree/TreeManager";
import { DeploymentScope, DeploymentScopeType, IAzureUiManager } from "./types";

function getSubscription(scope: DeploymentScope) {
  switch (scope.scopeType) {
    case "resourceGroup":
      return scope.subscription;
    case "subscription":
      return scope.subscription;
    case "managementGroup":
      return scope.associatedSubscription;
    case "tenant":
      return scope.associatedSubscription;
  }
}

export class AzureUiManager implements IAzureUiManager {
  public constructor(
    private readonly context: IActionContext,
    private readonly treeManager: TreeManager,
  ) {}

  public async getAccessToken(scope: DeploymentScope): Promise<AccessToken> {
    const subscription = getSubscription(scope);
    return createSubscriptionContext(subscription).credentials.getToken();
  }

  public async pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope> {
    switch (scopeType) {
      case "resourceGroup": { //asdfg test
        const subscription = await this.treeManager.pickSubscription(this.context);
        const resourceGroup = await this.treeManager.pickResourceGroup(this.context, subscription);

        return {
          scopeType,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          subscription,
          resourceGroup: resourceGroup.name!, //asdfg what if null?
        };
      }
      case "subscription": {
        const subscription = await this.treeManager.pickSubscription(this.context); //asdfg test
        const location = await this.treeManager.pickLocation(this.context, subscription);
        return {
          scopeType,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          subscription: subscription,
          location: location,
        };
      }
      case "managementGroup": throw new Error("Not implemented");
      case "tenant": throw new Error("Not implemented");
      // case "managementGroup": { //asdfg test
      //   const locationItem = await this.treeManager.azLocationTree.showTreeItemPicker("", this.context);
      //   const treeItem = await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker("", this.context);

      //   return {
      //     scopeType,
      //     portalUrl: locationItem.subscription.environment.portalUrl,
      //     tenantId: treeItem.subscription.tenantId,
      //     managementGroup: treeItem.label,
      //     associatedSubscriptionId: locationItem.subscription.subscriptionId,
      //     location: locationItem.label,
      //   };
      // }
      // case "tenant": { //asdfg test
      //   const locationItem = await this.treeManager.azLocationTree.showTreeItemPicker("", this.context);

      //   return {
      //     scopeType,
      //     portalUrl: locationItem.subscription.environment.portalUrl,
      //     tenantId: locationItem.subscription.tenantId,
      //     associatedSubscriptionId: locationItem.subscription.subscriptionId,
      //     location: locationItem.label,
      //   };
      // }
    }
  }
}
