// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { AccessToken } from "@azure/core-auth";
import { IActionContext, nonNullProp } from "../../../infrastructure/action-context";
import { DeploymentScope, DeploymentScopeType } from "../deployment-scope";
import { AzurePickers } from "./azure-pickers";
import { AzureSubscription, getAzureAccessToken } from "./azure-account-manager";

export interface IAzureUIManager {
  getAccessToken(scope: DeploymentScope): Promise<AccessToken>;
  pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope>;
}

export class AzureUIManager implements IAzureUIManager {
  public constructor(
    private readonly context: IActionContext,
    private readonly azurePickers: AzurePickers,
  ) {}

  public async getAccessToken(scope: DeploymentScope): Promise<AccessToken> {
    const subscription = await this.getSubscription(scope);
    return await getAzureAccessToken(subscription);
  }

  public async pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope> {
    switch (scopeType) {
      case "resourceGroup": {
        const subscription = await this.azurePickers.pickSubscription(this.context);
        const resourceGroup = await this.azurePickers.pickResourceGroup(this.context, subscription);

        return {
          scopeType,
          armUrl: subscription.environment.resourceManagerEndpointUrl,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          subscriptionId: subscription.subscriptionId,
          resourceGroup: nonNullProp(resourceGroup, "name"),
        };
      }
      case "subscription": {
        const subscription = await this.azurePickers.pickSubscription(this.context);
        const location = await this.azurePickers.pickLocation(this.context, subscription);
        return {
          scopeType,
          armUrl: subscription.environment.resourceManagerEndpointUrl,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          subscriptionId: subscription.subscriptionId,
          location: location,
        };
      }

      case "managementGroup": {
        const subscription = await this.azurePickers.pickSubscription(this.context);
        const managementGroup = await this.azurePickers.pickManagementGroup(this.context, subscription);
        const location = await this.azurePickers.pickLocation(this.context, subscription);

        return {
          scopeType,
          armUrl: subscription.environment.resourceManagerEndpointUrl,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          managementGroup: nonNullProp(managementGroup, "name"),
          associatedSubscriptionId: subscription.subscriptionId,
          location,
        };
      }

      case "tenant": {
        const subscription = await this.azurePickers.pickSubscription(this.context);
        const location = await this.azurePickers.pickLocation(this.context, subscription);

        return {
          scopeType,
          armUrl: subscription.environment.resourceManagerEndpointUrl,
          portalUrl: subscription.environment.portalUrl,
          tenantId: subscription.tenantId,
          associatedSubscriptionId: subscription.subscriptionId,
          location,
        };
      }
    }
  }

  private getSubscriptionId(scope: DeploymentScope): string {
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

  private async getSubscription(scope: DeploymentScope): Promise<AzureSubscription> {
    await this.azurePickers.ensureSignedIn();

    const subscriptionId = this.getSubscriptionId(scope);
    const subscriptions = await this.azurePickers.getAllSubscriptions();
    const subscription = subscriptions.find((s) => s.subscriptionId === subscriptionId);
    if (!subscription) {
      throw new Error(`Subscription with ID "${subscriptionId}" not found.`);
    }

    return subscription;
  }
}
