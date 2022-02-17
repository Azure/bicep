// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ResourceManagementClient } from "@azure/arm-resources";
import {
  AzExtClientContext,
  createAzureClient,
  parseClientContext,
} from "@microsoft/vscode-azext-azureutils";

// Lazy-load @azure packages to improve startup performance.
// NOTE: The client is the only import that matters, the rest of the types disappear when compiled to JavaScript

export async function createResourceClient(
  context: AzExtClientContext
): Promise<ResourceManagementClient> {
  if (parseClientContext(context).isCustomCloud) {
    return <ResourceManagementClient>(
      (<unknown>(
        createAzureClient(
          context,
          (await import("@azure/arm-resources-profile-2020-09-01-hybrid"))
            .ResourceManagementClient
        )
      ))
    );
  } else {
    return createAzureClient(
      context,
      (await import("@azure/arm-resources")).ResourceManagementClient
    );
  }
}
