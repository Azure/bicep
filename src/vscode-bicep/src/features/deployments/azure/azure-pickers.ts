// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ManagementGroupInfo as AzureManagementGroupInfo } from "@azure/arm-managementgroups";
import type { ResourceGroup as AzureResourceGroup, ResourceManagementClient } from "@azure/arm-resources" with {
  "resolution-mode": "import",
};

import { parseError } from "../../../infrastructure/errors";
import { Disposable } from "../../../infrastructure/lifecycle";
import { OutputChannelManager } from "../../../infrastructure/logging";
import { PromptItem, Prompts } from "../../../infrastructure/prompts";
import {
  createManagementGroupsClient,
  createResourceManagementClient,
  createSubscriptionClient,
} from "./azure-clients";
import { AzureAccountManager, AzureSubscription } from "./azure-account-manager";

const resourceGroupNamePattern = /^[\p{L}\p{Nd}_.()-]+$/u;

export interface SelectedResourceGroup {
  readonly id: string;
  readonly name: string;
}

export interface SelectedManagementGroup {
  readonly displayName?: string;
  readonly id: string;
  readonly name: string;
}

export class AzurePickers extends Disposable {
  private readonly accountManager = new AzureAccountManager();

  constructor(
    private readonly prompts: Prompts,
    private readonly outputChannelManager: OutputChannelManager,
  ) {
    super();
  }

  public async getAllSubscriptions(): Promise<AzureSubscription[]> {
    return await this.accountManager.getSubscriptions();
  }

  public async ensureSignedIn(): Promise<void> {
    if (await this.accountManager.isSignedIn()) {
      return;
    }

    await this.accountManager.signIn();
  }

  public async pickSubscription(): Promise<AzureSubscription> {
    await this.ensureSignedIn();

    const subscriptions = await this.getAllSubscriptions();
    if (subscriptions.length === 0) {
      throw new Error(`No subscriptions found. ${await this.getTenantInfo()}`);
    }

    subscriptions.sort((a, b) => a.name.localeCompare(b.name));

    const picks = subscriptions.map((s) => {
      return <PromptItem<AzureSubscription>>{
        label: s.name,
        description: s.subscriptionId,
        data: s,
      };
    });

    return (await this.prompts.showQuickPick(picks, { placeHolder: "Select subscription" })).data;
  }

  public async pickResourceGroup(subscription: AzureSubscription): Promise<SelectedResourceGroup> {
    await this.ensureSignedIn();

    const client: ResourceManagementClient = await createResourceManagementClient(subscription);
    const resourceGroups: SelectedResourceGroup[] = [];
    for (const resourceGroup of await listAll(client.resourceGroups.list())) {
      try {
        resourceGroups.push(toSelectedResourceGroup(resourceGroup));
      } catch (error) {
        this.outputChannelManager.appendToOutputChannel(parseError(error).message);
      }
    }
    resourceGroups.sort((a, b) => a.name.localeCompare(b.name));

    const createNewRGItem: PromptItem<SelectedResourceGroup | undefined> = {
      label: "$(plus) Create new resource group",
      data: undefined,
    };

    const picks = [
      createNewRGItem,
      ...resourceGroups.map((resourceGroup): PromptItem<SelectedResourceGroup | undefined> => ({
        label: resourceGroup.name,
        data: resourceGroup,
      })),
    ];

    const selected = await this.prompts.showQuickPick(picks, { placeHolder: "Select resource group" });
    if (selected === createNewRGItem) {
      return await this.promptCreateResourceGroup(subscription);
    }

    if (!selected.data) {
      throw new Error("The selected resource group is missing its value.");
    }

    return selected.data;
  }

  public async pickLocation(subscription: AzureSubscription): Promise<string> {
    await this.ensureSignedIn();

    const client = await createSubscriptionClient(subscription);
    const locations = (await listAll(client.subscriptions.listLocations(subscription.subscriptionId))).flatMap(
      (location) => (location.name ? [location.name] : []),
    );
    locations.sort();

    const picks = locations.map(
      (l) =>
        <PromptItem<string>>{
          label: l,
          data: l,
        },
    );

    return (await this.prompts.showQuickPick(picks, { placeHolder: "Select location" })).data;
  }

  public async pickManagementGroup(subscription: AzureSubscription): Promise<SelectedManagementGroup> {
    await this.ensureSignedIn();

    const client = await createManagementGroupsClient(subscription);

    let response: AzureManagementGroupInfo[];
    try {
      response = await listAll(client.managementGroups.list());
    } catch (err) {
      throw new Error(
        `You might not have access to any management groups. Please create one in the Azure portal and try to deploy again.  Error: ${parseError(err).message}. ${await this.getTenantInfo()}`,
        { cause: err },
      );
    }

    const managementGroups: SelectedManagementGroup[] = [];
    for (const managementGroup of response) {
      try {
        managementGroups.push(toSelectedManagementGroup(managementGroup));
      } catch (error) {
        this.outputChannelManager.appendToOutputChannel(parseError(error).message);
      }
    }
    managementGroups.sort((a, b) => (a.displayName ?? a.name).localeCompare(b.displayName ?? b.name));

    const picks = managementGroups.map(
      (managementGroup) =>
        <PromptItem<SelectedManagementGroup>>{
          label: managementGroup.displayName ?? managementGroup.name,
          description: managementGroup.name,
          data: managementGroup,
        },
    );

    return (await this.prompts.showQuickPick(picks, { placeHolder: "Select management group" })).data;
  }

  private async promptCreateResourceGroup(subscription: AzureSubscription): Promise<SelectedResourceGroup> {
    const resourceGroupName = (
      await this.prompts.showInputBox({
        title: "Create resource group",
        prompt: "Enter a resource group name",
        validateInput: validateResourceGroupName,
      })
    ).trim();
    const location = await this.pickLocation(subscription);
    const client = await createResourceManagementClient(subscription);
    const exists = await client.resourceGroups.checkExistence(resourceGroupName);
    if (exists.body) {
      throw new Error(`A resource group named '${resourceGroupName}' already exists.`);
    }

    const resourceGroup = toSelectedResourceGroup(
      await client.resourceGroups.createOrUpdate(resourceGroupName, { location }),
    );

    this.outputChannelManager.appendToOutputChannel(`Created resource group "${resourceGroupName}"`);
    return resourceGroup;
  }

  private async getTenantInfo(): Promise<string> {
    try {
      const tenants = await this.accountManager.getTenants();
      const signInStatusPromises = tenants.map(async (tenant) => {
        const isSignedIn = await this.accountManager.isSignedIn(tenant.tenantId, tenant.account);
        return `${tenant.tenantId} (${isSignedIn ? "signed in" : "signed out"})`;
      });
      const signInStatus = await Promise.all(signInStatusPromises);
      return ` Available tenants: ${signInStatus.join(", ")}`;
    } catch (err) {
      this.outputChannelManager.appendToOutputChannel(parseError(err).message);
      return "Unable to retrieve available tenant information.";
    }
  }
}

export function validateResourceGroupName(value: string): string | undefined {
  const name = value.trim();
  if (name.length === 0) {
    return "A resource group name is required.";
  }

  if (name.length > 90) {
    return "A resource group name cannot exceed 90 characters.";
  }

  if (!resourceGroupNamePattern.test(name)) {
    return "A resource group name can contain only letters, digits, underscores, hyphens, periods, and parentheses.";
  }

  if (name.endsWith(".")) {
    return "A resource group name cannot end with a period.";
  }

  return undefined;
}

async function listAll<T>(items: AsyncIterable<T>): Promise<T[]> {
  const results: T[] = [];
  for await (const item of items) {
    results.push(item);
  }

  return results;
}

function toSelectedResourceGroup(resourceGroup: AzureResourceGroup): SelectedResourceGroup {
  if (!resourceGroup.id || !resourceGroup.name) {
    throw new Error("Azure returned a resource group without an ID or name.");
  }

  return { id: resourceGroup.id, name: resourceGroup.name };
}

function toSelectedManagementGroup(managementGroup: AzureManagementGroupInfo): SelectedManagementGroup {
  if (!managementGroup.id || !managementGroup.name) {
    throw new Error("Azure returned a management group without an ID or name.");
  }

  return {
    displayName: managementGroup.displayName,
    id: managementGroup.id,
    name: managementGroup.name,
  };
}
