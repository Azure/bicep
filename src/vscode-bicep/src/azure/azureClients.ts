// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { ResourceManagementClient } from "@azure/arm-resources";
import { SubscriptionClient } from "@azure/arm-resources-subscriptions";
import {
  AzExtClientContext,
  createAzureClient,
  createAzureSubscriptionClient,
} from "@microsoft/vscode-azext-azureutils";

// Lazy-load @azure packages to improve startup performance.

export async function createResourceManagementClient(
  context: AzExtClientContext
): Promise<ResourceManagementClient> {
  return createAzureClient(
    context,
    (await import("@azure/arm-resources")).ResourceManagementClient
  );
}

export async function createSubscriptionClient(
  context: AzExtClientContext
): Promise<SubscriptionClient> {
  return createAzureSubscriptionClient(
    context,
    (await import("@azure/arm-resources-subscriptions")).SubscriptionClient
  );
}
