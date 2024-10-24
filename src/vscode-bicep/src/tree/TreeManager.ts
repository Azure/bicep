// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
//asdfg import { AzExtTreeDataProvider } from "@microsoft/vscode-azext-utils";
import { AzExtTreeDataProvider, createSubscriptionContext, IActionContext, IAzureQuickPickItem, nonNullProp, parseError } from "@microsoft/vscode-azext-utils"; //asdfg remove
import { Disposable } from "../utils/disposable";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { AzManagementGroupTreeItem } from "./AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "./AzResourceGroupTreeItem";
import { AzureSubscription, VSCodeAzureSubscriptionProvider } from "@microsoft/vscode-azext-azureauth";
import { ResourceGroup, ResourceManagementClient } from "@azure/arm-resources";
import { createResourceManagementClient, createSubscriptionClient } from "../azure/azureClients";
import { uiUtils } from "@microsoft/vscode-azext-azureutils";
//import { QuickInputButton } from "vscode";

//asdfg filters

//asdfg
// const signInLabel: string = l10n.t('Sign in to Azure...');
// const createAccountLabel: string = l10n.t('Create an Azure Account...');
//const createStudentAccountLabel: string = 'Create an Azure for Students Account...';
// const selectSubscriptionsLabel: string = l10n.t('Select Subscriptions...');
// const signInCommandId: string = 'azure-account.login';
// const createAccountCommandId: string = 'azure-account.createAccount';
// const createStudentAccountCommandId: string = 'azure-account.createStudentAccount';
// const selectSubscriptionsCommandId: string = 'azure-account.selectSubscriptions';
// const azureAccountExtensionId: string = 'ms-vscode.azure-account';
// const extensionOpenCommand: string = 'extension.open';

export class TreeManager extends Disposable {
  private vsCodeAzureSubscriptionProvider = new VSCodeAzureSubscriptionProvider();

  constructor(private readonly outputChannelManager: OutputChannelManager) {
    super();
  }

  get azManagementGroupTreeItem(): AzExtTreeDataProvider { //asdfg remove these
    const azManagementGroupTreeItem: AzManagementGroupTreeItem = this.register(new AzManagementGroupTreeItem());
    return new AzExtTreeDataProvider(azManagementGroupTreeItem, "");
  }

  get azResourceGroupTreeItem(): AzExtTreeDataProvider {
    const azResourceGroupTreeItem: AzResourceGroupTreeItem = this.register(
      new AzResourceGroupTreeItem(this.outputChannelManager),
    );
    return new AzExtTreeDataProvider(azResourceGroupTreeItem, "");
  }

  public async EnsureSignedIn(): Promise<void> {
    if (await this.vsCodeAzureSubscriptionProvider.isSignedIn()) {
      return;
    }

    // asdfg progress?
    await this.vsCodeAzureSubscriptionProvider.signIn();

    //asdfg let azureAccount: AzureAccountResult = await this._azureAccountTask;
    // if (typeof azureAccount === 'string') {
    //     // Refresh the AzureAccount, to handle Azure account extension installation after the previous refresh
    //     this._azureAccountTask = this.loadAzureAccount(this._testAccount);
    //     azureAccount = await this._azureAccountTask;
    // }

    // if (typeof azureAccount === 'string') {
    //     context.telemetry.properties.accountStatus = azureAccount;
    //     const label: string = azureAccount === 'notInstalled' ?
    //         l10n.t('Install Azure Account Extension...') :
    //         l10n.t('Update Azure Account Extension to at least version "{0}"...', minAccountExtensionVersion);
    //     const iconPath: TreeItemIconPath = new ThemeIcon('warning');
    //     const result: AzExtTreeItem = new GenericTreeItem(this, { label, commandId: extensionOpenCommand, contextValue: 'azureAccount' + azureAccount, includeInTreeItemPicker: true, iconPath });
    //     result.commandArgs = [azureAccountExtensionId];
    //     return [result];
    // }

    //asdfg context.telemetry.properties.accountStatus = azureAccount.status;

    //asdfg need telemetry about using new api?

    //asdfg const existingSubscriptions: SubscriptionTreeItemBase[] = this._subscriptionTreeItems ? this._subscriptionTreeItems : [];
    // this._subscriptionTreeItems = [];


    //asdfg const contextValue: string = 'azureCommand';



    //asdfg
    // if (azureAccount.status === 'Initializing' || azureAccount.status === 'LoggingIn') {
    //   return [new GenericTreeItem(this, {
    //       label: azureAccount.status === 'Initializing' ? l10n.t('Loading...') : l10n.t('Waiting for Azure sign-in...'),
    //       commandId: signInCommandId,
    //       contextValue,
    //       id: signInCommandId,
    //       iconPath: new ThemeIcon('loading~spin')
    //   })];

    // asdfg use AzLoginTreeItem code

    // const studentAccountTreeItem:IAzureQuickPickItem ={
    //       label: createStudentAccountLabel,
    //       commandId: 'azureResourceGroups.openUrl',
    //       contextValue,
    //       id: createStudentAccountCommandId,
    //       iconPath: new ThemeIcon('mortar-board'),
    //       includeInTreeItemPicker: true
    //   });

    //   studentAccountTreeItem.commandArgs = ['https://aka.ms/student-account'];

    //   const signInItem:IAzureQuickPickItem = {

    //   }
    //   return [
    //       new GenericTreeItem(this, { label: signInLabel, commandId: signInCommandId, contextValue, id: signInCommandId, iconPath: new ThemeIcon('sign-in'), includeInTreeItemPicker: true }),
    //       new GenericTreeItem(this, { label: createAccountLabel, commandId: createAccountCommandId, contextValue, id: createAccountCommandId, iconPath: new ThemeIcon('add'), includeInTreeItemPicker: true }),
    //       studentAccountTreeItem
    //   ];

    /*asdfg
    if (azureAccount.status === 'Initializing' || azureAccount.status === 'LoggingIn') {
        return [new GenericTreeItem(this, {
            label: azureAccount.status === 'Initializing' ? l10n.t('Loading...') : l10n.t('Waiting for Azure sign-in...'),
            commandId: signInCommandId,
            contextValue,
            id: signInCommandId,
            iconPath: new ThemeIcon('loading~spin')
        })];
    } else if (azureAccount.status === 'LoggedOut') {
        const studentAccountTreeItem = new GenericTreeItem(this, {
            label: createStudentAccountLabel,
            commandId: 'azureResourceGroups.openUrl',
            contextValue,
            id: createStudentAccountCommandId,
            iconPath: new ThemeIcon('mortar-board'),
            includeInTreeItemPicker: true
        });

        studentAccountTreeItem.commandArgs = ['https://aka.ms/student-account'];

        return [
            new GenericTreeItem(this, { label: signInLabel, commandId: signInCommandId, contextValue, id: signInCommandId, iconPath: new ThemeIcon('sign-in'), includeInTreeItemPicker: true }),
            new GenericTreeItem(this, { label: createAccountLabel, commandId: createAccountCommandId, contextValue, id: createAccountCommandId, iconPath: new ThemeIcon('add'), includeInTreeItemPicker: true }),
            studentAccountTreeItem
        ];
    }*/

    //asdfg await azureAccount.waitForFilters();

    //asdfg ???  this._vsCodeAzureSubscriptionProvider.getTenants


    /*asdfg
    if (azureAccount.filters.length === 0) {
        return [
            new GenericTreeItem(this, { label: selectSubscriptionsLabel, commandId: selectSubscriptionsCommandId, contextValue, id: selectSubscriptionsCommandId, includeInTreeItemPicker: true })
        ];
    } else {*/

    //asdfg .isSignedIn()
  }

  public async pickSubscription(context: IActionContext): Promise<AzureSubscription> {
    await this.EnsureSignedIn();

    const subscriptions = await this.vsCodeAzureSubscriptionProvider.getSubscriptions();
    if (subscriptions.length === 0) {
      throw new Error("No subscriptions found");
    }

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
    const picks = rgs.map((rg) => {
      try {
        return <IAzureQuickPickItem<ResourceGroup>>{
          label: nonNullProp(rg, "name"),
          data: rg,
        };
      } catch (error) {
        this.outputChannelManager.appendToOutputChannel(parseError(error).message);
        return undefined;
      }
    }).filter(p => !!p);

    return (await context.ui.showQuickPick(picks, { placeHolder: "Select resource group" })).data;
  }

  public async pickLocation(context: IActionContext, subscription: AzureSubscription): Promise<string> {
    const client = await createSubscriptionClient([context, createSubscriptionContext(subscription)]);
    const locations = (await uiUtils.listAllIterator(client.subscriptions.listLocations(subscription.subscriptionId))).map(l => l.name);
    const picks = locations.map((l) => <IAzureQuickPickItem<string>>{
        label: l,
        data: l
      });    
    
    return (await context.ui.showQuickPick(picks, { placeHolder: "Select location" })).data;
  }
}

