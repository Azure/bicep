// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";

type DeploymentScopeBase<T> = {
  armUrl: string;
  portalUrl: string;
  tenantId: string;
} & T;

export type DeploymentScope = DeploymentScopeBase<
  | {
      scopeType: "resourceGroup";
      subscriptionId: string;
      resourceGroup: string;
    }
  | {
      scopeType: "subscription";
      location: string;
      subscriptionId: string;
    }
  | {
      scopeType: "managementGroup";
      associatedSubscriptionId: string;
      location: string;
      managementGroup: string;
    }
  | {
      scopeType: "tenant";
      associatedSubscriptionId: string;
      location: string;
    }
>;

export type DeploymentScopeType = "resourceGroup" | "subscription" | "managementGroup" | "tenant";

export interface IAzureUiManager {
  getAccessToken(scope: DeploymentScope): Promise<AccessToken>;
  pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope>;
}
