// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { addExtensionValueToMask, AzExtParentTreeItem, AzExtServiceClientCredentials, AzExtTreeItem, AzureWizardPromptStep, GenericTreeItem, IActionContext, ISubscriptionActionContext, ISubscriptionContext, nonNullProp, nonNullValue, registerEvent, TreeItemIconPath, UserCancelledError } from '@microsoft/vscode-azext-utils';
import * as semver from 'semver';
import { commands, Disposable, Extension, extensions, l10n, MessageItem, ProgressLocation, ThemeIcon, window } from 'vscode';
//asdfg import * as types from '../../index';
import { AzureAccountExtensionApi, AzureLoginStatus, AzureResourceFilter } from '../azure-account.api';
//asdfg? import { getIconPath } from './IconPath';
import { SubscriptionTreeItemBase } from './SubscriptionTreeItemBase';

//asdfg new/copied

const signInLabel: string = l10n.t('Sign in to Azure...');
const createAccountLabel: string = l10n.t('Create an Azure Account...');
const createStudentAccountLabel: string = l10n.t('Create an Azure for Students Account...');
const selectSubscriptionsLabel: string = l10n.t('Select Subscriptions...');
const signInCommandId: string = 'azure-account.login';
const createAccountCommandId: string = 'azure-account.createAccount';
const createStudentAccountCommandId: string = 'azure-account.createStudentAccount';
const selectSubscriptionsCommandId: string = 'azure-account.selectSubscriptions';
const azureAccountExtensionId: string = 'ms-vscode.azure-account';
const extensionOpenCommand: string = 'extension.open';

type AzureAccountResult = AzureAccountExtensionApi | 'notInstalled' | 'needsUpdate';
const minAccountExtensionVersion: string = '0.9.0';

export abstract class AzureAccountTreeItemBase extends AzExtParentTreeItem {
    public static contextValue: string = 'azureextensionui.azureAccount';
    public readonly contextValue: string = AzureAccountTreeItemBase.contextValue;
    public readonly label: string = 'Azure';
    public childTypeLabel: string = l10n.t('subscription');
    public autoSelectInTreeItemPicker: boolean = true;
    public disposables: Disposable[] = [];
    public suppressMaskLabel: boolean = true;

    private _azureAccountTask: Promise<AzureAccountResult>;
    private _subscriptionTreeItems: SubscriptionTreeItemBase[] | undefined;
    private _testAccount: AzureAccountExtensionApi | undefined;

    constructor(parent?: AzExtParentTreeItem, testAccount?: AzureAccountExtensionApi) {
        super(parent);
        this._testAccount = testAccount;
        this._azureAccountTask = this.loadAzureAccount(testAccount);
    }

    //#region Methods implemented by base class
    public abstract createSubscriptionTreeItem(root: ISubscriptionContext): SubscriptionTreeItemBase | Promise<SubscriptionTreeItemBase>;
    //#endregion

    public get iconPath(): TreeItemIconPath {
        return getIconPath('azure');
    }

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
                l10n.t('Install Azure Account Extension...') :
                l10n.t('Update Azure Account Extension to at least version "{0}"...', minAccountExtensionVersion);
            const iconPath: TreeItemIconPath = new ThemeIcon('warning');
            const result: AzExtTreeItem = new GenericTreeItem(this, { label, commandId: extensionOpenCommand, contextValue: 'azureAccount' + azureAccount, includeInTreeItemPicker: true, iconPath });
            result.commandArgs = [azureAccountExtensionId];
            return [result];
        }

        context.telemetry.properties.accountStatus = azureAccount.status;
        const existingSubscriptions: SubscriptionTreeItemBase[] = this._subscriptionTreeItems ? this._subscriptionTreeItems : [];
        this._subscriptionTreeItems = [];

        const contextValue: string = 'azureCommand';
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
        }

        await azureAccount.waitForFilters();

        if (azureAccount.filters.length === 0) {
            return [
                new GenericTreeItem(this, { label: selectSubscriptionsLabel, commandId: selectSubscriptionsCommandId, contextValue, id: selectSubscriptionsCommandId, includeInTreeItemPicker: true })
            ];
        } else {
            this._subscriptionTreeItems = await Promise.all(azureAccount.filters.map(async (filter: AzureResourceFilter) => {
                const existingTreeItem: SubscriptionTreeItemBase | undefined = existingSubscriptions.find(ti => ti.id === filter.subscription.id);
                if (existingTreeItem) {
                    // Return existing treeItem (which might have many 'cached' tree items underneath it) rather than creating a brand new tree item every time
                    return existingTreeItem;
                } else {
                    addExtensionValueToMask(
                        filter.subscription.id,
                        filter.subscription.subscriptionId,
                        filter.subscription.displayName,
                        filter.session.userId,
                        filter.session.tenantId,
                    );

                    // these properties don't exist on TokenCredentials, but do exist on DeviceTokenCredentials
                    // addExtensionValueToMask gracefully handles `undefined` input by ignoring it
                    addExtensionValueToMask(
                        (filter.session.credentials2 as unknown as { clientId: string | undefined }).clientId,
                        (filter.session.credentials2 as unknown as { domain: string | undefined }).domain
                    );

                    // filter.subscription.id is the The fully qualified ID of the subscription (For example, /subscriptions/00000000-0000-0000-0000-000000000000) and should be used as the tree item's id for the purposes of OpenInPortal
                    // filter.subscription.subscriptionId is just the guid and is used in all other cases when creating clients for managing Azure resources
                    const subscriptionId: string = nonNullProp(filter.subscription, 'subscriptionId');
                    return await this.createSubscriptionTreeItem({
                        credentials: <AzExtServiceClientCredentials>filter.session.credentials2,
                        createCredentialsForScopes: () => { return Promise.resolve(filter.session.credentials2) },
                        subscriptionDisplayName: nonNullProp(filter.subscription, 'displayName'),
                        subscriptionId,
                        subscriptionPath: nonNullProp(filter.subscription, 'id'),
                        tenantId: filter.session.tenantId,
                        userId: filter.session.userId,
                        environment: filter.session.environment,
                        isCustomCloud: filter.session.environment.name === 'AzureCustomCloud'
                    });
                }
            }));
            return this._subscriptionTreeItems;
        }
    }

    public async getIsLoggedIn(): Promise<boolean> {
        const azureAccount: AzureAccountResult = await this._azureAccountTask;
        return typeof azureAccount !== 'string' && azureAccount.status === 'LoggedIn';
    }

    public async getSubscriptionPromptStep(context: Partial<ISubscriptionActionContext> & IActionContext): Promise<AzureWizardPromptStep<ISubscriptionActionContext> | undefined> {
        const subscriptionNodes: SubscriptionTreeItemBase[] = await this.ensureSubscriptionTreeItems(context);
        if (subscriptionNodes.length === 1) {
            Object.assign(context, subscriptionNodes[0].subscription);
            return undefined;
        } else {
            // eslint-disable-next-line @typescript-eslint/no-this-alias
            const me: AzureAccountTreeItemBase = this;
            class SubscriptionPromptStep extends AzureWizardPromptStep<ISubscriptionActionContext> {
                public async prompt(): Promise<void> {
                    const ti: SubscriptionTreeItemBase = <SubscriptionTreeItemBase>await me.treeDataProvider.showTreeItemPicker(SubscriptionTreeItemBase.contextValue, context, me);
                    Object.assign(context, ti.subscription);
                }
                public shouldPrompt(): boolean { return !(<ISubscriptionActionContext>context).subscriptionId; }
            }
            return new SubscriptionPromptStep();
        }
    }

    public async pickTreeItemImpl(_expectedContextValues: (string | RegExp)[]): Promise<AzExtTreeItem | undefined> {
        const azureAccount: AzureAccountResult = await this._azureAccountTask;
        if (typeof azureAccount !== 'string' && (azureAccount.status === 'LoggingIn' || azureAccount.status === 'Initializing')) {
            const title: string = l10n.t('Waiting for Azure sign-in...');
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

    private async loadAzureAccount(azureAccount: AzureAccountExtensionApi | undefined): Promise<AzureAccountResult> {
        if (!azureAccount) {
            const extension: Extension<AzureAccountExtensionApi> | undefined = extensions.getExtension<AzureAccountExtensionApi>(azureAccountExtensionId);
            if (extension) {
                try {
                    // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
                    if (semver.lt(extension.packageJSON.version as string, minAccountExtensionVersion)) {
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
            registerEvent('azureAccount.onStatusChanged', azureAccount.onStatusChanged, async (context: IActionContext, status: AzureLoginStatus) => {
                context.errorHandling.suppressDisplay = true;
                context.telemetry.suppressIfSuccessful = true;
                // Ignore status change to 'LoggedIn' and wait for the 'onFiltersChanged' event to fire instead
                // (so that the tree stays in 'Loading...' state until the filters are actually ready)
                if (status !== 'LoggedIn') {
                    await this.refresh(context);
                }
            });
            await commands.executeCommand('setContext', 'isAzureAccountInstalled', true);
            return azureAccount;
        } else {
            return 'notInstalled';
        }
    }

    private async ensureSubscriptionTreeItems(context: IActionContext): Promise<SubscriptionTreeItemBase[]> {
        const azureAccount: AzureAccountResult = await this._azureAccountTask;
        if (typeof azureAccount === 'string') {
            let message: string;
            let stepName: string;
            if (azureAccount === 'notInstalled') {
                stepName = 'requiresAzureAccount';
                message = l10n.t("This functionality requires installing the Azure Account extension.");
            } else {
                stepName = 'requiresUpdateToAzureAccount';
                message = l10n.t('This functionality requires updating the Azure Account extension to at least version "{0}".', minAccountExtensionVersion);
            }

            const viewInMarketplace: MessageItem = { title: l10n.t("View in Marketplace") };
            if (await context.ui.showWarningMessage(message, { stepName }, viewInMarketplace) === viewInMarketplace) {
                await commands.executeCommand(extensionOpenCommand, azureAccountExtensionId);
            }

            throw new UserCancelledError(`${stepName}|viewInMarketplace`);
        }

        if (!this._subscriptionTreeItems) {
            await this.getCachedChildren(context);
        }

        return nonNullValue(this._subscriptionTreeItems, 'subscriptionTreeItems');
    }
}
