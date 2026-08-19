// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceManagementClient } from "@azure/arm-resources" with { "resolution-mode": "import" };

import { ManagementGroupInfo } from "@azure/arm-managementgroups";
import { ResourceGroup } from "@azure/arm-resources";
import { IActionContext, IAzureQuickPickItem, nonNullProp, parseError } from "../../../infrastructure/action-context";
import { Disposable } from "../../../infrastructure/lifecycle";
import { OutputChannelManager } from "../../../infrastructure/logging";
import {
  createManagementGroupsClient,
  createResourceManagementClient,
  createSubscriptionClient,
} from "./azure-clients";
import { AzureAccountManager, AzureSubscription } from "./azure-account-manager";

const resourceGroupNamePattern = /^[\p{L}\p{Nd}_.()-]+$/u;

export class AzurePickers extends Disposable {
  private readonly accountManager = new AzureAccountManager();

  constructor(private readonly outputChannelManager: OutputChannelManager) {
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

  public async pickSubscription(context: IActionContext): Promise<AzureSubscription> {
    await this.ensureSignedIn();

    const subscriptions = await this.getAllSubscriptions();
    if (subscriptions.length === 0) {
      throw new Error(`No subscriptions found. ${await this.getTenantInfo()}`);
    }

    subscriptions.sort((a, b) => a.name.localeCompare(b.name));

    const picks = subscriptions.map((s) => {
      return <IAzureQuickPickItem<AzureSubscription>>{
        label: s.name,
        description: s.subscriptionId,
        data: s,
      };
    });

    return (await context.ui.showQuickPick(picks, { placeHolder: "Select subscription" })).data;
  }

  public async pickResourceGroup(context: IActionContext, subscription: AzureSubscription): Promise<ResourceGroup> {
    await this.ensureSignedIn();

    const client: ResourceManagementClient = await createResourceManagementClient(subscription);
    const rgs = await listAll(client.resourceGroups.list());

    rgs.sort((a, b) => nonNullProp(a, "name").localeCompare(nonNullProp(b, "name")));

    const createNewRGItem: IAzureQuickPickItem<ResourceGroup | undefined> = {
      label: "$(plus) Create new resource group",
      data: undefined,
    };

    const picks = [
      createNewRGItem,
      ...rgs
        .map((rg) => {
          try {
            return <IAzureQuickPickItem<ResourceGroup | undefined>>{
              label: nonNullProp(rg, "name"),
              data: rg,
            };
          } catch (error) {
            this.outputChannelManager.appendToOutputChannel(parseError(error).message);
            return undefined;
          }
        })
        .filter((p) => !!p),
    ];

    const selected = await context.ui.showQuickPick(picks, { placeHolder: "Select resource group" });
    if (selected === createNewRGItem) {
      return await this.promptCreateResourceGroup(context, subscription);
    } else {
      return selected.data!;
    }
  }

  public async pickLocation(context: IActionContext, subscription: AzureSubscription): Promise<string> {
    await this.ensureSignedIn();

    const client = await createSubscriptionClient(subscription);
    const locations = (await listAll(client.subscriptions.listLocations(subscription.subscriptionId))).map((l) =>
      nonNullProp(l, "name"),
    );
    locations.sort();

    const picks = locations.map(
      (l) =>
        <IAzureQuickPickItem<string>>{
          label: l,
          data: l,
        },
    );

    return (await context.ui.showQuickPick(picks, { placeHolder: "Select location" })).data;
  }

  public async pickManagementGroup(
    context: IActionContext,
    subscription: AzureSubscription,
  ): Promise<ManagementGroupInfo> {
    await this.ensureSignedIn();

    const client = await createManagementGroupsClient(subscription);

    let managementGroups: ManagementGroupInfo[];
    try {
      managementGroups = await listAll(client.managementGroups.list());
    } catch (err) {
      throw new Error(
        `You might not have access to any management groups. Please create one in the Azure portal and try to deploy again.  Error: ${parseError(err).message}. ${await this.getTenantInfo()}`,
        { cause: err },
      );
    }

    managementGroups.sort((a, b) =>
      (a.displayName ?? nonNullProp(a, "name")).localeCompare(b.displayName ?? nonNullProp(b, "name")),
    );

    const picks = managementGroups.map(
      (mg) =>
        <IAzureQuickPickItem<ManagementGroupInfo>>{
          label: mg.displayName ?? mg.name,
          description: mg.name,
          data: mg,
        },
    );

    return (await context.ui.showQuickPick(picks, { placeHolder: "Select management group" })).data;
  }

  private async promptCreateResourceGroup(
    context: IActionContext,
    subscription: AzureSubscription,
  ): Promise<ResourceGroup> {
    const resourceGroupName = (
      await context.ui.showInputBox({
        title: "Create resource group",
        prompt: "Enter a resource group name",
        validateInput: validateResourceGroupName,
      })
    ).trim();
    const location = await this.pickLocation(context, subscription);
    const client = await createResourceManagementClient(subscription);
    const exists = await client.resourceGroups.checkExistence(resourceGroupName);
    if (exists.body) {
      throw new Error(`A resource group named '${resourceGroupName}' already exists.`);
    }

    const resourceGroup = await client.resourceGroups.createOrUpdate(resourceGroupName, { location });

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
