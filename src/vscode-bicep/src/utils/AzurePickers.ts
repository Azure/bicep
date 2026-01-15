// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ManagementGroupInfo } from "@azure/arm-managementgroups";
import { ResourceGroup, ResourceManagementClient } from "@azure/arm-resources";
import { AzureSubscription, VSCodeAzureSubscriptionProvider } from "@microsoft/vscode-azext-azureauth";
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
import {
  createManagementGroupsClient,
  createResourceManagementClient,
  createSubscriptionClient,
} from "../azure/azureClients";
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

  public async EnsureSignedIn(): Promise<void> {
    if (await this.vsCodeAzureSubscriptionProvider.isSignedIn()) {
      return;
    }

    await this.vsCodeAzureSubscriptionProvider.signIn();
  }

  public async pickSubscription(context: IActionContext): Promise<AzureSubscription> {
    await this.EnsureSignedIn();

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
    await this.EnsureSignedIn();

    const subscriptionContext = createSubscriptionContext(subscription);
    const client: ResourceManagementClient = await createResourceManagementClient([context, subscriptionContext]);
    const rgs: ResourceGroup[] = await uiUtils.listAllIterator(client.resourceGroups.list());

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
    await this.EnsureSignedIn();

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

  public async pickManagementGroup(
    context: IActionContext,
    subscription: AzureSubscription,
  ): Promise<ManagementGroupInfo> {
    await this.EnsureSignedIn();

    const client = await createManagementGroupsClient([context, createSubscriptionContext(subscription)]);

    let managementGroups: ManagementGroupInfo[];
    try {
      managementGroups = await uiUtils.listAllIterator(client.managementGroups.list());
    } catch (err) {
      throw new Error(
        `You might not have access to any management groups. Please create one in the Azure portal and try to deploy again.  Error: ${parseError(err).message}. ${await this.getTenantInfo()}`,
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

    const azTreeItem = nonNullProp(wizardContext, "resourceGroup");
    const newResourceGroupName = nonNullProp(azTreeItem, "name");

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

  private async getTenantInfo(): Promise<string> {
    try {
      const tenants = await this.vsCodeAzureSubscriptionProvider.getTenants();
      const signInStatusPromises = tenants.map(async (tenant) => {
        const isSignedIn = await this.vsCodeAzureSubscriptionProvider.isSignedIn(tenant.tenantId);
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
