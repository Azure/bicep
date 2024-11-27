// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ManagementGroupInfo, ManagementGroupsAPI } from "@azure/arm-managementgroups";
import { ResourceGroup, ResourceManagementClient } from "@azure/arm-resources";
import { DefaultAzureCredential } from "@azure/identity";
import { AzureSubscription, VSCodeAzureSubscriptionProvider } from "@microsoft/vscode-azext-azureauth";
import type { TenantIdDescription } from '@azure/arm-resources-subscriptions';
import {
  IResourceGroupWizardContext,
  LocationListStep,
  ResourceGroupCreateStep,
  ResourceGroupNameStep,
  uiUtils,
} from "@microsoft/vscode-azext-azureutils";
import {
  AzureWizard,
  AzureWizardExecuteStep,
  AzureWizardPromptStep,
  createSubscriptionContext,
  IActionContext,
  IAzureQuickPickItem,
  nonNullProp,
  parseError,
} from "@microsoft/vscode-azext-utils";
import { createResourceManagementClient, createSubscriptionClient } from "../azure/azureClients";
import { Disposable } from "./disposable";
import { OutputChannelManager } from "./OutputChannelManager";

export class AzurePickers extends Disposable {
  private vsCodeAzureSubscriptionProvider = new VSCodeAzureSubscriptionProvider();

  constructor(private readonly outputChannelManager: OutputChannelManager) {
    super();
  }

  public async getAllSubscriptions(): Promise<AzureSubscription[]> {
    return await this.vsCodeAzureSubscriptionProvider.getSubscriptions(false);
  }

  public async getAllTenants(): Promise<TenantIdDescription[]> {
    return await this.vsCodeAzureSubscriptionProvider.getTenants(); //asdfg account?
  }

  // Call this before calling any of the pickers or getAll* functions
  public async EnsureSignedIn(context: IActionContext): Promise<void> {
    if (!await this.vsCodeAzureSubscriptionProvider.isSignedIn()) {
      await this.vsCodeAzureSubscriptionProvider.signIn();
    }

    if ((await this.getAllSubscriptions()).length === 0) {
      context.telemetry.properties.subscriptionCount = "0"; //asdfg
      this.outputChannelManager.appendToOutputChannel(`No currently-accessible subscriptions found. ${await this.getTenantInfo()}`);

      var tenant = await this.pickTenant(context);
      context.telemetry.properties.subscriptionCount = "0"; //asdfg
      this.vsCodeAzureSubscriptionProvider.signIn(tenant.tenantId); //asdfg what if cancelled?
    }
  }

  private async pickTenant(context: IActionContext): Promise<TenantIdDescription> {
    const tenants = await this.getTenantsAndSignedInStatus();
    if (tenants.length === 0) {
      throw new Error("No currently-accessible tenants found.");
    }

    if (tenants.length === 1) {
      return tenants[0].tenant;
    }

    tenants.sort((a, b) => this.getTenantLabel(a.tenant).localeCompare(this.getTenantLabel(b.tenant)));

    const pickPromises = tenants.map((t) => {
      return <IAzureQuickPickItem<TenantIdDescription>>{
        label: this.getTenantLabel(t.tenant) + (t.isSignedIn ? " (Signed In)" : ""),
        description: t.tenant.tenantId,
        data: t,
      };
    });

    var picks = await Promise.all(pickPromises);
    return (await context.ui.showQuickPick(picks, { placeHolder: "Select tenant" })).data;
  }

  public async pickSubscription(context: IActionContext): Promise<AzureSubscription> {
    const subscriptions = await this.getAllSubscriptions();
    if (subscriptions.length === 0) {
      throw new Error(`No currently-accessible subscriptions found. ${await this.getTenantInfo()}`);
    }

    if (subscriptions.length === 1) {
      return subscriptions[0];
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
    const subscriptionContext = createSubscriptionContext(subscription);
    const client: ResourceManagementClient = await createResourceManagementClient([context, subscriptionContext]);
    const rgs: ResourceGroup[] = await uiUtils.listAllIterator(client.resourceGroups.list());

    rgs.sort((a, b) => this.getResourceGroupLabel(a).localeCompare(this.getResourceGroupLabel(b)));

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
              label: this.getResourceGroupLabel(rg),
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
    const client = await createSubscriptionClient([context, createSubscriptionContext(subscription)]);
    const locations = (
      await uiUtils.listAllIterator(client.subscriptions.listLocations(subscription.subscriptionId))
    ).map((l) => nonNullProp(l, "name"));
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

  public async pickManagementGroup(context: IActionContext): Promise<ManagementGroupInfo> {
    const managementGroupsAPI = new ManagementGroupsAPI(new DefaultAzureCredential());
    let managementGroups: ManagementGroupInfo[];
    try {
      managementGroups = await uiUtils.listAllIterator(managementGroupsAPI.managementGroups.list());
    } catch (err) {
      throw new Error(
        `You might not have access to any management groups. Please create one in the Azure portal and try to deploy again.  Error: ${parseError(err).message}. ${await this.getTenantInfo()}`,
      );
    }

    managementGroups.sort((a, b) =>
      (a.displayName ?? this.getManagementGroupLabel(a)).localeCompare(this.getManagementGroupLabel(b)),
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
    const subscriptionContext = createSubscriptionContext(subscription);
    const wizardContext: IResourceGroupWizardContext = {
      ...context,
      ...subscriptionContext,
      ...subscription,
      suppress403Handling: true,
    };
    const promptSteps: AzureWizardPromptStep<IResourceGroupWizardContext>[] = [new ResourceGroupNameStep()];
    LocationListStep.addStep(wizardContext, promptSteps);
    const executeSteps: AzureWizardExecuteStep<IResourceGroupWizardContext>[] = [new ResourceGroupCreateStep()];

    const wizard: AzureWizard<IResourceGroupWizardContext> = new AzureWizard(wizardContext, {
      title: "Create Resource Group",
      promptSteps,
      executeSteps,
    });

    await wizard.prompt();
    await wizard.execute();

    const rg = nonNullProp(wizardContext, "resourceGroup");
    const newResourceGroupName = nonNullProp(rg, "name");

    this.outputChannelManager.appendToOutputChannel(`Created resource group "${newResourceGroupName}"`);

    const client: ResourceManagementClient = await createResourceManagementClient([context, subscriptionContext]);
    const rgs: ResourceGroup[] = await uiUtils.listAllIterator(client.resourceGroups.list());
    const newResourceGroup =
      rgs.find((rg) => rg.name === newResourceGroupName) ??
      (() => {
        throw new Error(`Failed to find newly created resource group "${newResourceGroupName}"`);
      })();
    return newResourceGroup;
  }

  private async getTenantsAndSignedInStatus(): Promise<{ tenant: TenantIdDescription; isSignedIn: boolean }[]> {
    const tenants = await this.vsCodeAzureSubscriptionProvider.getTenants();
    const promises = tenants.map(async (tenant) => {
      const isSignedIn = await this.vsCodeAzureSubscriptionProvider.isSignedIn(tenant.tenantId);
      return { tenant, isSignedIn }
    });

    return await Promise.all(promises);
  }

  private async getTenantInfo(): Promise<string> {
    try {
      const tenants = await this.getTenantsAndSignedInStatus();
      var signInStatus = tenants.map(tenant => `${tenant.tenant.tenantId} (${tenant.isSignedIn ? "signed in" : "signed out"})`);
      return ` Available tenants: ${signInStatus.join(", ")}`;
    } catch (err) {
      this.outputChannelManager.appendToOutputChannel(parseError(err).message);
      return "Unable to retrieve available tenant information.";
    }
  }

  private getTenantLabel(tenant: TenantIdDescription): string {
    return tenant.displayName ?? nonNullProp(tenant, "tenantId");
  }

  private getResourceGroupLabel(rg: ResourceGroup): string {
    return nonNullProp(rg, "name");
  }

  private getManagementGroupLabel(mg: ManagementGroupInfo): string {
    return mg.displayName ?? nonNullProp(mg, "name");
  }
}
