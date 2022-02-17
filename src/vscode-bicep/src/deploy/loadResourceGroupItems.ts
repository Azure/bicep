// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode from "vscode";
import { AzureAccount } from "../azure-account.api";
import { IActionContext, IAzureQuickPickItem } from "@microsoft/vscode-azext-utils";
import {
  ResourceManagementModels,
  ResourceManagementClient,
} from "@azure/arm-resources";

export async function loadResourceGroupItems(
  context: IActionContext,
  subscriptionId: string
): Promise<
  IAzureQuickPickItem<ResourceManagementModels.ResourceGroup | string>[]
> {
  const resources = await getResourceGroupItems(subscriptionId);

  resources.push({
    label: "$(plus) Create new Resource Group...",
    data: "createResourceGroup",
  });

  return resources;
}

//export async function loadResourceGroupItems(
//  subscriptionId: string
//) {
//  const resources = await getResourceGroups(subscriptionId);

//  return resources;
//}

export async function loadResourceGroups(
  context: IActionContext,
  subscriptionId: string
) {
  const resources = await getResourceGroups(subscriptionId);

  return resources;
}

async function getResourceGroupItems(subscriptionId: string) {
  const azureAccount = vscode.extensions.getExtension<AzureAccount>(
    "ms-vscode.azure-account"
  )!.exports;
  const session = azureAccount.sessions[0];

  const resources = new ResourceManagementClient(
    session.credentials2,
    subscriptionId
  );
  const resourceGroups = await listAll(
    resources.resourceGroups,
    resources.resourceGroups.list()
  );
  return resourceGroups.map((resourceGroup) =>
    getResourceGroupQuickPick(resourceGroup)
  );
}

async function getResourceGroups(subscriptionId: string) {
  const azureAccount = vscode.extensions.getExtension<AzureAccount>(
    "ms-vscode.azure-account"
  )!.exports;
  const session = azureAccount.sessions[0];

  const resources = new ResourceManagementClient(
    session.credentials2,
    subscriptionId
  );
  const resourceGroups = await listAll(
    resources.resourceGroups,
    resources.resourceGroups.list()
  );
  return resourceGroups;
}

export function getResourceGroupQuickPick(
  resourceGroup: ResourceManagementModels.ResourceGroup
): IAzureQuickPickItem<ResourceManagementModels.ResourceGroup> {
  return { label: resourceGroup.name, data: resourceGroup.id };
}

async function listAll<T>(
  client: { listNext(nextPageLink: string): Promise<PartialList<T>> },
  first: Promise<PartialList<T>>
): Promise<T[]> {
  const all: T[] = [];
  for (
    let list = await first;
    list.length || list.nextLink;
    list = list.nextLink ? await client.listNext(list.nextLink) : []
  ) {
    all.push(...list);
  }
  return all;
}

export interface PartialList<T> extends Array<T> {
  nextLink?: string;
}
