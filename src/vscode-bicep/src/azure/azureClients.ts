// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import type { ManagementGroupsAPI } from "@azure/arm-managementgroups" with { "resolution-mode": "import" };
import type { ResourceManagementClient } from "@azure/arm-resources" with { "resolution-mode": "import" };
import type { SubscriptionClient } from "@azure/arm-resources-subscriptions" with { "resolution-mode": "import" };

import { AzureSubscription } from "@microsoft/vscode-azext-azureauth";
import { appendExtensionUserAgent } from "@microsoft/vscode-azext-utils";

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
    endpoint: subscription.environment.resourceManagerEndpointUrl,
    userAgentOptions: {
      userAgentPrefix: appendExtensionUserAgent(),
    },
  };
}
