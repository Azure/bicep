// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ResourceGroup, ResourceManagementClient } from "@azure/arm-resources";
import {
  IResourceGroupWizardContext,
  LocationListStep,
  ResourceGroupCreateStep,
  ResourceGroupNameStep,
  SubscriptionTreeItemBase,
  uiUtils,
} from "@microsoft/vscode-azext-azureutils";
import {
  AzExtTreeItem,
  AzureWizard,
  AzureWizardExecuteStep,
  AzureWizardPromptStep,
  IActionContext,
  ICreateChildImplContext,
  nonNullProp,
} from "@microsoft/vscode-azext-utils";
import { createResourceClient } from "../utils/azureClients";
import { localize } from "../utils/localize";
import { ResourceGroupTreeItem } from "./ResourceGroupTreeItem";

export class SubscriptionTreeItem extends SubscriptionTreeItemBase {
  public readonly childTypeLabel: string = localize(
    "resourceGroup",
    "Resource Group"
  );

  private _nextLink: string | undefined;

  public hasMoreChildrenImpl(): boolean {
    return !!this._nextLink;
  }

  public async loadMoreChildrenImpl(
    clearCache: boolean,
    context: IActionContext
  ): Promise<AzExtTreeItem[]> {
    //if (clearCache) {
    //  this._nextLink = undefined;
    //}
    //const client: ResourceManagementClient = await createResourceClient([
    //  context,
    //  this,
    //]);
    //const rgs: ResourceManagementModels.ResourceGroupListResult = this._nextLink
    //  ? await client.resourceGroups.listNext(this._nextLink)
    //  : await client.resourceGroups.list();
    //this._nextLink = rgs.nextLink;
    //const resourceTreeItems = await this.createTreeItemsWithErrorHandling(
    //  rgs,
    //  "invalidResourceGroup",
    //  (rg) => new ResourceGroupTreeItem(this, rg),
    //  (rg) => rg.name
    //);
    //return resourceTreeItems;

    if (clearCache) {
      this._nextLink = undefined;
    }

    const client: ResourceManagementClient = await createResourceClient([
      context,
      this,
    ]);
    // Load more currently broken https://github.com/Azure/azure-sdk-for-js/issues/20380
    const rgs: ResourceGroup[] = await uiUtils.listAllIterator(
      client.resourceGroups.list()
    );
    const resourceGroupItems = await this.createTreeItemsWithErrorHandling(
      rgs,
      "invalidResourceGroup",
      (rg) => new ResourceGroupTreeItem(this, rg),
      (rg) => rg.name
    );

    return resourceGroupItems;
  }

  public async createChildImpl(
    context: ICreateChildImplContext
  ): Promise<AzExtTreeItem> {
    const title: string = localize(
      "createResourceGroup",
      "Create Resource Group"
    );
    const wizardContext: IResourceGroupWizardContext = {
      ...context,
      ...this.subscription,
      suppress403Handling: true,
    };
    const promptSteps: AzureWizardPromptStep<IResourceGroupWizardContext>[] = [
      new ResourceGroupNameStep(),
    ];
    LocationListStep.addStep(wizardContext, promptSteps);
    const executeSteps: AzureWizardExecuteStep<IResourceGroupWizardContext>[] =
      [new ResourceGroupCreateStep()];

    const wizard: AzureWizard<IResourceGroupWizardContext> = new AzureWizard(
      wizardContext,
      { title, promptSteps, executeSteps }
    );
    await wizard.prompt();
    context.showCreatingTreeItem(
      nonNullProp(wizardContext, "newResourceGroupName")
    );
    await wizard.execute();

    return new ResourceGroupTreeItem(
      this,
      nonNullProp(wizardContext, "resourceGroup")
    );
  }
}
