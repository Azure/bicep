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

import { appendToOutputChannel } from "../../utils/appendToOutputChannel";
import { localize } from "../../utils/localize";
import { createResourceManagementClient } from "../azureClients";
import { GenericAzExtTreeItem } from "./GenericAzExtTreeItem";

// Represents an Azure subscription. Used to display resource groups related to the subscription
export class ResourceGroupTreeItem extends SubscriptionTreeItemBase {
  public readonly childTypeLabel: string = localize(
    "resourceGroup",
    "Resource Group"
  );

  private _nextLink: string | undefined;

  public hasMoreChildrenImpl(): boolean {
    return !!this._nextLink;
  }

  // Loads resource group
  public async loadMoreChildrenImpl(
    clearCache: boolean,
    context: IActionContext
  ): Promise<AzExtTreeItem[]> {
    if (clearCache) {
      this._nextLink = undefined;
    }
    const client: ResourceManagementClient =
      await createResourceManagementClient([context, this]);
    const rgs: ResourceGroup[] = await uiUtils.listAllIterator(
      client.resourceGroups.list()
    );
    const resourceGroupItems = await this.createTreeItemsWithErrorHandling(
      rgs,
      "invalidResourceGroup",
      (rg) => new GenericAzExtTreeItem(this, rg.id, rg.name),
      (rg) => rg.name
    );

    return resourceGroupItems;
  }

  // Adds 'create' option in the resource group tree picker
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

    const azTreeItem = nonNullProp(wizardContext, "resourceGroup");
    const newResourceGroupItemName = azTreeItem.name;
    const newResourceGroupItem = new GenericAzExtTreeItem(
      this,
      azTreeItem.id,
      newResourceGroupItemName
    );

    appendToOutputChannel(
      `Created resource group -> ${newResourceGroupItemName}`
    );

    return newResourceGroupItem;
  }
}
