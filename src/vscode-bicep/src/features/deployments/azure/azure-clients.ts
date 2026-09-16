// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { ManagementGroupsAPI } from "@azure/arm-managementgroups" with { "resolution-mode": "import" };
import type { ResourceManagementClient } from "@azure/arm-resources" with { "resolution-mode": "import" };
import type { SubscriptionClient } from "@azure/arm-resources-subscriptions" with { "resolution-mode": "import" };

import { extensions } from "vscode";
import { AzureSubscription } from "./azure-account-manager";
import { getAzureResourceManagerClientOptions } from "./azure-environment";

// Lazy-load @azure packages to improve startup performance.

export async function createResourceManagementClient(
  subscription: AzureSubscription,
): Promise<ResourceManagementClient> {
  const { ResourceManagementClient } = await import("@azure/arm-resources");
  return new ResourceManagementClient(
    subscription.credential,
    subscription.subscriptionId,
    getClientOptions(subscription),
  );
}

export async function createSubscriptionClient(subscription: AzureSubscription): Promise<SubscriptionClient> {
  const { SubscriptionClient } = await import("@azure/arm-resources-subscriptions");
  return new SubscriptionClient(subscription.credential, getClientOptions(subscription));
}

export async function createManagementGroupsClient(subscription: AzureSubscription): Promise<ManagementGroupsAPI> {
  const { ManagementGroupsAPI } = await import("@azure/arm-managementgroups");
  return new ManagementGroupsAPI(subscription.credential, getClientOptions(subscription));
}

function getClientOptions(subscription: AzureSubscription) {
  return {
    ...getAzureResourceManagerClientOptions(subscription.environment),
    userAgentOptions: {
      userAgentPrefix: getExtensionUserAgent(),
    },
  };
}

function getExtensionUserAgent(): string {
  const extension = extensions.getExtension("ms-azuretools.vscode-bicep");
  const packageJson = extension?.packageJSON as { name?: string; version?: string } | undefined;
  return `${packageJson?.name ?? "vscode-bicep"}/${packageJson?.version ?? "unknown"}`;
}
