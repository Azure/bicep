/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import * as semver from 'semver';
import { commands, Disposable, Extension, extensions, ProgressLocation, ThemeIcon, window } from 'vscode';
//import * as types from '../../index';
import { AzureAccount, AzureLoginStatus } from '../azure-account.api';
//import { UserCancelledError } from '../errors';
import { localize } from '../ui/localize';
import { registerEvent } from '../ui/registerEvent';
//import { addExtensionValueToMask } from '../masking';
//import { registerEvent } from '../registerEvent';
//import { nonNullProp, nonNullValue } from '../utils/nonNull';
//import { AzureWizardPromptStep } from '../wizard/AzureWizardPromptStep';
//import { AzExtParentTreeItem } from './AzExtParentTreeItem';
//import { GenericTreeItem } from './GenericTreeItem';
//import { getIconPath } from './IconPath';
//import { SubscriptionTreeItemBase } from './SubscriptionTreeItemBase';
//import { DeviceTokenCredentials } from '@azure/ms-rest-nodeauth';
import { EmptyTreeItem } from './EmptyTreeItem';
import { AzExtTreeItem, AzExtParentTreeItem, GenericTreeItem, IActionContext } from 'vscode-azureextensionui';


const signInLabel: string = localize('signInLabel', 'Sign in to Azure...');
const createAccountLabel: string = localize('createAccountLabel', 'Create a Free Azure Account...');
//const selectSubscriptionsLabel: string = localize('noSubscriptions', 'Select Subscriptions...');
const signInCommandId: string = 'azure-account.login';
const createAccountCommandId: string = 'azure-account.createAccount';
//const selectSubscriptionsCommandId: string = 'azure-account.selectSubscriptions';
const azureAccountExtensionId: string = 'ms-vscode.azure-account';
const extensionOpenCommand: string = 'extension.open';

type AzureAccountResult = AzureAccount | 'notInstalled' | 'needsUpdate';
const minAccountExtensionVersion: string = '0.9.0';

export abstract class AzureAccountTreeItemBase extends AzExtParentTreeItem {
  public static contextValue: string = 'azureextensionui.azureAccount';
  public readonly contextValue: string = AzureAccountTreeItemBase.contextValue;
  public readonly label: string = 'Azure';
  public childTypeLabel: string = localize('subscription', 'subscription');
  public autoSelectInTreeItemPicker: boolean = true;
  public disposables: Disposable[] = [];
  public suppressMaskLabel: boolean = true;

  private _azureAccountTask: Promise<AzureAccountResult>;
  //private _subscriptionTreeItems: SubscriptionTreeItemBase[] | undefined;
  private _testAccount: AzureAccount | undefined;

  constructor(parent?: AzExtParentTreeItem, testAccount?: AzureAccount) {
    super(parent);
    this._testAccount = testAccount;
    this._azureAccountTask = this.loadAzureAccount(testAccount);
  }

  //#region Methods implemented by base class
  //public abstract createSubscriptionTreeItem(root: types.ISubscriptionContext): SubscriptionTreeItemBase | Promise<SubscriptionTreeItemBase>;
  //#endregion

  //public get iconPath(): types.TreeItemIconPath {
  //  return getIconPath('azure');
  //}

  public dispose(): void {
    Disposable.from(...this.disposables).dispose();
  }

  public hasMoreChildrenImpl(): boolean {
    return false;
  }

  public async loadMoreChildrenImpl(_clearCache: boolean, context: IActionContext): Promise<AzExtTreeItem[]> {
    let azureAccount: AzureAccountResult = await this._azureAccountTask;
    if (typeof azureAccount === 'string') {
      // Refresh the AzureAccount, to handle Azure account extension installation after the previous refresh
      this._azureAccountTask = this.loadAzureAccount(this._testAccount);
      azureAccount = await this._azureAccountTask;
    }

    if (typeof azureAccount === 'string') {
      context.telemetry.properties.accountStatus = azureAccount;
      const label: string = azureAccount === 'notInstalled' ?
        localize('installAzureAccount', 'Install Azure Account Extension...') :
        localize('updateAzureAccount', 'Update Azure Account Extension to at least version "{0}"...', minAccountExtensionVersion);
      //const iconPath: types.TreeItemIconPath = new ThemeIcon('warning');
      const result: AzExtTreeItem = new GenericTreeItem(this, { label, commandId: extensionOpenCommand, contextValue: 'azureAccount' + azureAccount, includeInTreeItemPicker: true });
      result.commandArgs = [azureAccountExtensionId];
      return [result];
    }

    //context.telemetry.properties.accountStatus = azureAccount.status;
   // const existingSubscriptions: SubscriptionTreeItemBase[] = this._subscriptionTreeItems ? this._subscriptionTreeItems : [];
   // this._subscriptionTreeItems = [];

    const contextValue: string = 'azureCommand';
    if (azureAccount.status === 'Initializing' || azureAccount.status === 'LoggingIn') {
      return [new GenericTreeItem(this, {
        label: azureAccount.status === 'Initializing' ? localize('loadingTreeItem', 'Loading...') : localize('signingIn', 'Waiting for Azure sign-in...'),
        commandId: signInCommandId,
        contextValue,
        id: signInCommandId,
        iconPath: new ThemeIcon('loading~spin')
      })];
    } else if (azureAccount.status === 'LoggedOut') {
      return [
        new GenericTreeItem(this, { label: signInLabel, commandId: signInCommandId, contextValue, id: signInCommandId, iconPath: new ThemeIcon('sign-in'), includeInTreeItemPicker: true }),
        new GenericTreeItem(this, { label: createAccountLabel, commandId: createAccountCommandId, contextValue, id: createAccountCommandId, iconPath: new ThemeIcon('add'), includeInTreeItemPicker: true })
      ];
    }

    return [new EmptyTreeItem(this)];

    //await azureAccount.waitForFilters();

    //if (azureAccount.filters.length === 0) {
    //  return [
    //    new GenericTreeItem(this, { label: selectSubscriptionsLabel, commandId: selectSubscriptionsCommandId, contextValue, id: selectSubscriptionsCommandId, includeInTreeItemPicker: true })
    //  ];
    //}
    //else {
    //  this._subscriptionTreeItems = await Promise.all(azureAccount.filters.map(async (filter: AzureResourceFilter) => {
    //    const existingTreeItem: SubscriptionTreeItemBase | undefined = existingSubscriptions.find(ti => ti.id === filter.subscription.id);
    //    if (existingTreeItem) {
    //      // Return existing treeItem (which might have many 'cached' tree items underneath it) rather than creating a brand new tree item every time
    //      return existingTreeItem;
    //    } else {
    //      addExtensionValueToMask(
    //        filter.subscription.id,
    //        filter.subscription.subscriptionId,
    //        filter.subscription.displayName,
    //        filter.session.userId,
    //        filter.session.tenantId,
    //      );

    //      // these properties don't exist on TokenCredentials
    //      if (filter.session.credentials2 instanceof DeviceTokenCredentials) {
    //        addExtensionValueToMask(
    //          filter.session.credentials2.clientId,
    //          filter.session.credentials2.domain);
    //      }

    //      // filter.subscription.id is the The fully qualified ID of the subscription (For example, /subscriptions/00000000-0000-0000-0000-000000000000) and should be used as the tree item's id for the purposes of OpenInPortal
    //      // filter.subscription.subscriptionId is just the guid and is used in all other cases when creating clients for managing Azure resources
    //      const subscriptionId: string = nonNullProp(filter.subscription, 'subscriptionId');
    //      return await this.createSubscriptionTreeItem({
    //        credentials: <types.AzExtServiceClientCredentials>filter.session.credentials2,
    //        subscriptionDisplayName: nonNullProp(filter.subscription, 'displayName'),
    //        subscriptionId,
    //        subscriptionPath: nonNullProp(filter.subscription, 'id'),
    //        tenantId: filter.session.tenantId,
    //        userId: filter.session.userId,
    //        environment: filter.session.environment,
    //        isCustomCloud: filter.session.environment.name === 'AzureCustomCloud'
    //      });
    //    }
    //  }));
    //  return this._subscriptionTreeItems;
    //}
  }

  public async getIsLoggedIn(): Promise<boolean> {
    const azureAccount: AzureAccountResult = await this._azureAccountTask;
    return typeof azureAccount !== 'string' && azureAccount.status === 'LoggedIn';
  }

  //public async getSubscriptionPromptStep(context: Partial<types.ISubscriptionActionContext> & types.IActionContext): Promise<types.AzureWizardPromptStep<types.ISubscriptionActionContext> | undefined> {
  //  const subscriptionNodes: SubscriptionTreeItemBase[] = await this.ensureSubscriptionTreeItems(context);
  //  if (subscriptionNodes.length === 1) {
  //    Object.assign(context, subscriptionNodes[0].subscription);
  //    return undefined;
  //  } else {
  //    // eslint-disable-next-line @typescript-eslint/no-this-alias
  //    const me: AzureAccountTreeItemBase = this;
  //    class SubscriptionPromptStep extends AzureWizardPromptStep<types.ISubscriptionActionContext> {
  //      public async prompt(): Promise<void> {
  //        const ti: SubscriptionTreeItemBase = <SubscriptionTreeItemBase>await me.treeDataProvider.showTreeItemPicker(SubscriptionTreeItemBase.contextValue, context, me);
  //        Object.assign(context, ti.subscription);
  //      }
  //      public shouldPrompt(): boolean { return !(<types.ISubscriptionActionContext>context).subscriptionId; }
  //    }
  //    return new SubscriptionPromptStep();
  //  }
  //}

  public async pickTreeItemImpl(_expectedContextValues: (string | RegExp)[]): Promise<AzExtTreeItem | undefined> {
    const azureAccount: AzureAccountResult = await this._azureAccountTask;
    if (typeof azureAccount !== 'string' && (azureAccount.status === 'LoggingIn' || azureAccount.status === 'Initializing')) {
      const title: string = localize('waitingForAzureSignin', 'Waiting for Azure sign-in...');
      // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
      await window.withProgress({ location: ProgressLocation.Notification, title }, async (): Promise<boolean> => await azureAccount!.waitForSubscriptions());
    }

    return undefined;
  }

  public compareChildrenImpl(item1: AzExtTreeItem, item2: AzExtTreeItem): number {
    if (item1 instanceof GenericTreeItem && item2 instanceof GenericTreeItem) {
      return 0; // already sorted
    } else {
      return super.compareChildrenImpl(item1, item2);
    }
  }

  private async loadAzureAccount(azureAccount: AzureAccount | undefined): Promise<AzureAccountResult> {
    if (!azureAccount) {
      const extension: Extension<AzureAccount> | undefined = extensions.getExtension<AzureAccount>(azureAccountExtensionId);
      if (extension) {
        try {
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
          if (semver.lt(extension.packageJSON.version, minAccountExtensionVersion)) {
            return 'needsUpdate';
          }
        } catch {
          // ignore and assume extension is up to date
        }

        if (!extension.isActive) {
          await extension.activate();
        }

        azureAccount = extension.exports;
      }
    }

    if (azureAccount) {
      registerEvent('azureAccount.onFiltersChanged', azureAccount.onFiltersChanged, async (context) => {
        context.errorHandling.suppressDisplay = true;
        context.telemetry.suppressIfSuccessful = true;
        await this.refresh(context);
      });
      //registerEvent('azureAccount.onStatusChanged', azureAccount.onStatusChanged, async (context: IActionContext, status: AzureLoginStatus) => {
      //  context.errorHandling.suppressDisplay = true;
      //  context.telemetry.suppressIfSuccessful = true;
      //  // Ignore status change to 'LoggedIn' and wait for the 'onFiltersChanged' event to fire instead
      //  // (so that the tree stays in 'Loading...' state until the filters are actually ready)
      //  if (status !== 'LoggedIn') {
      //    await this.refresh(context);
      //  }
      //});
      await commands.executeCommand('setContext', 'isAzureAccountInstalled', true);
      return azureAccount;
    } else {
      return 'notInstalled';
    }
  }

//  private async ensureSubscriptionTreeItems(context: types.IActionContext): Promise<SubscriptionTreeItemBase[]> {
//    const azureAccount: AzureAccountResult = await this._azureAccountTask;
//    if (typeof azureAccount === 'string') {
//      let message: string;
//      let stepName: string;
//      if (azureAccount === 'notInstalled') {
//        stepName = 'requiresAzureAccount';
//        message = localize('requiresAzureAccount', "This functionality requires installing the Azure Account extension.");
//      } else {
//        stepName = 'requiresUpdateToAzureAccount';
//        message = localize('requiresUpdateToAzureAccount', 'This functionality requires updating the Azure Account extension to at least version "{0}".', minAccountExtensionVersion);
//      }

//      const viewInMarketplace: MessageItem = { title: localize('viewInMarketplace', "View in Marketplace") };
//      if (await context.ui.showWarningMessage(message, { stepName }, viewInMarketplace) === viewInMarketplace) {
//        await commands.executeCommand(extensionOpenCommand, azureAccountExtensionId);
//      }

//      throw new UserCancelledError(`${stepName}|viewInMarketplace`);
//    }

//    if (!this._subscriptionTreeItems) {
//      await this.getCachedChildren(context);
//    }

//    return nonNullValue(this._subscriptionTreeItems, 'subscriptionTreeItems');
//  }
}
