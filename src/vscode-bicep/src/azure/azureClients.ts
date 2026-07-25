// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ManagementGroupsAPI } from "@azure/arm-managementgroups";
import { ResourceManagementClient } from "@azure/arm-resources";
import { SubscriptionClient } from "@azure/arm-resources-subscriptions";
import { TokenCredential } from "@azure/core-auth";
import {
  AzExtClientContext,
  createAzureSubscriptionClient,
  parseClientContext,
} from "@microsoft/vscode-azext-azureutils";

// Lazy-load @azure packages to improve startup performance.

export async function createResourceManagementClient(context: AzExtClientContext): Promise<ResourceManagementClient> {
  const parsedContext = parseClientContext(context);
  return new ResourceManagementClient(
    parsedContext.credentials as TokenCredential,
    parsedContext.subscriptionId,
    {
      endpoint: parsedContext.environment.resourceManagerEndpointUrl,
    },
  );
}

export async function createSubscriptionClient(context: AzExtClientContext): Promise<SubscriptionClient> {
  return createAzureSubscriptionClient(
    context,
    (await import("@azure/arm-resources-subscriptions")).SubscriptionClient,
  );
}

export async function createManagementGroupsClient(context: AzExtClientContext): Promise<ManagementGroupsAPI> {
  return createAzureSubscriptionClient(context, (await import("@azure/arm-managementgroups")).ManagementGroupsAPI);
}
