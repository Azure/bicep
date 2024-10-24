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

export async function createResourceManagementClient(context: AzExtClientContext): Promise<ResourceManagementClient> {
  return createAzureClient(context, (await import("@azure/arm-resources")).ResourceManagementClient);//asdfg
}

export async function createSubscriptionClient(context: AzExtClientContext): Promise<SubscriptionClient> {
  // asdfg note: * 2. Uses resourceManagerEndpointUrl to support sovereigns
  return createAzureSubscriptionClient( //asdfg
    context,
    (await import("@azure/arm-resources-subscriptions")).SubscriptionClient,
  );
}
