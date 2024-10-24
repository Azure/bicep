// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { AccessToken } from "@azure/identity";
import { AzureSubscription } from "@microsoft/vscode-azext-azureauth";
import { Event } from "vscode";

// https://github.com/microsoft/vscode-azure-account/blob/main/src/azure-account.api.d.ts //asdfg
// with just the properties we need
export interface AzureAccount {
  readonly status: AzureLoginStatus;
  readonly onStatusChanged: Event<AzureLoginStatus>;
}

export type AzureLoginStatus = "Initializing" | "LoggingIn" | "LoggedIn" | "LoggedOut";

type DeploymentScopeBase<T> = {
  portalUrl: string;
  tenantId: string;
} & T;

export type DeploymentScope = DeploymentScopeBase<
  | {
    scopeType: "resourceGroup";
    subscription: AzureSubscription;
    resourceGroup: string;
  }
  | {
    scopeType: "subscription";
    location: string;
    subscription: AzureSubscription;
  }
  | {
    scopeType: "managementGroup";
    associatedSubscription: AzureSubscription;
    location: string;
    managementGroup: string;
  }
  | {
    scopeType: "tenant";
    associatedSubscription: AzureSubscription;
    location: string;
  }
>;

export type DeploymentScopeType = "resourceGroup" | "subscription" | "managementGroup" | "tenant";

export interface IAzureUiManager {
  getAccessToken(scope: DeploymentScope): Promise<AccessToken>;
  pickScope(scopeType: DeploymentScopeType): Promise<DeploymentScope>;
}
