// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import { Event } from "vscode";

// https://github.com/microsoft/vscode-azure-account/blob/main/src/azure-account.api.d.ts
// with just the properties we need
export interface AzureAccount {
  readonly status: AzureLoginStatus;
  readonly onStatusChanged: Event<AzureLoginStatus>;
}

export type AzureLoginStatus =
  | "Initializing"
  | "LoggingIn"
  | "LoggedIn"
  | "LoggedOut";

type DeploymentScopeBase = {
  portalUrl: string;
  tenantId: string;
};

export type DeploymentScope =
  | (DeploymentScopeBase & {
      scopeType: "resourceGroup";
      subscriptionId: string;
      resourceGroup: string;
    })
  | (DeploymentScopeBase & {
      scopeType: "subscription";
      location: string;
      subscriptionId: string;
    })
  | (DeploymentScopeBase & {
      scopeType: "managementGroup";
      associatedSubscriptionId: string;
      location: string;
      managementGroup: string;
    })
  | (DeploymentScopeBase & {
      scopeType: "tenant";
      associatedSubscriptionId: string;
      location: string;
    });

export type DeploymentScopeType =
  | "resourceGroup"
  | "subscription"
  | "managementGroup"
  | "tenant";

export interface IAzureUiManager {
  getAccessToken(scope: DeploymentScope): Promise<AccessToken>;
  pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope>;
}