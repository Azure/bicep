// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as semver from "semver";
import { commands, Disposable, Extension, extensions, ThemeIcon } from "vscode";
import { AzureAccount } from "../../azure-account.api";
import { localize } from "../../utils/localize";
import {
  registerEvent,
  AzExtTreeItem,
  AzExtParentTreeItem,
  GenericTreeItem,
  IActionContext,
} from "@microsoft/vscode-azext-utils";
import { GenericAzExtTreeItem } from "./GenericAzExtTreeItem";

const signInLabel: string = localize("signInLabel", "Sign in to Azure...");
const createAccountLabel: string = localize(
  "createAccountLabel",
  "Create a Free Azure Account..."
);
const signInCommandId = "azure-account.login";
const createAccountCommandId = "azure-account.createAccount";
const azureAccountExtensionId = "ms-vscode.azure-account";
const extensionOpenCommand = "extension.open";

type AzureAccountResult = AzureAccount | "notInstalled" | "needsUpdate";
const minAccountExtensionVersion = "0.9.0";

export abstract class AzureAccountTreeItemBase extends AzExtParentTreeItem {
  public static contextValue = "azureextensionui.azureAccount";
  public readonly contextValue: string = AzureAccountTreeItemBase.contextValue;
  public readonly label: string = "Azure";
  public childTypeLabel: string = localize("subscription", "subscription");
  public autoSelectInTreeItemPicker = true;
  public disposables: Disposable[] = [];
  public suppressMaskLabel = true;

  private _azureAccountTask: Promise<AzureAccountResult>;
  private _testAccount: AzureAccount | undefined;

  constructor(parent?: AzExtParentTreeItem, testAccount?: AzureAccount) {
    super(parent);
    this._testAccount = testAccount;
    this._azureAccountTask = this.loadAzureAccount(testAccount);
  }

  public dispose(): void {
    Disposable.from(...this.disposables).dispose();
  }

  public hasMoreChildrenImpl(): boolean {
    return false;
  }

  public async loadMoreChildrenImpl(
    _clearCache: boolean,
    context: IActionContext
  ): Promise<AzExtTreeItem[]> {
    let azureAccount: AzureAccountResult = await this._azureAccountTask;
    if (typeof azureAccount === "string") {
      // Refresh the AzureAccount, to handle Azure account extension installation after the previous refresh
      this._azureAccountTask = this.loadAzureAccount(this._testAccount);
      azureAccount = await this._azureAccountTask;
    }

    if (typeof azureAccount === "string") {
      context.telemetry.properties.accountStatus = azureAccount;
      const label: string =
        azureAccount === "notInstalled"
          ? localize(
              "installAzureAccount",
              "Install Azure Account Extension..."
            )
          : localize(
              "updateAzureAccount",
              'Update Azure Account Extension to at least version "{0}"...',
              minAccountExtensionVersion
            );
      const result: AzExtTreeItem = new GenericTreeItem(this, {
        label,
        commandId: extensionOpenCommand,
        contextValue: "azureAccount" + azureAccount,
        includeInTreeItemPicker: true,
      });
      result.commandArgs = [azureAccountExtensionId];
      return [result];
    }

    const contextValue = "azureCommand";
    if (
      azureAccount.status === "Initializing" ||
      azureAccount.status === "LoggingIn"
    ) {
      return [
        new GenericTreeItem(this, {
          label:
            azureAccount.status === "Initializing"
              ? localize("loadingTreeItem", "Loading...")
              : localize("signingIn", "Waiting for Azure sign-in..."),
          commandId: signInCommandId,
          contextValue,
          id: signInCommandId,
          iconPath: new ThemeIcon("loading~spin"),
        }),
      ];
    } else if (azureAccount.status === "LoggedOut") {
      return [
        new GenericTreeItem(this, {
          label: signInLabel,
          commandId: signInCommandId,
          contextValue,
          id: signInCommandId,
          iconPath: new ThemeIcon("sign-in"),
          includeInTreeItemPicker: true,
        }),
        new GenericTreeItem(this, {
          label: createAccountLabel,
          commandId: createAccountCommandId,
          contextValue,
          id: createAccountCommandId,
          iconPath: new ThemeIcon("add"),
          includeInTreeItemPicker: true,
        }),
      ];
    }

    return [new GenericAzExtTreeItem(this, "", "")];
  }

  private async loadAzureAccount(
    azureAccount: AzureAccount | undefined
  ): Promise<AzureAccountResult> {
    if (!azureAccount) {
      const extension: Extension<AzureAccount> | undefined =
        extensions.getExtension<AzureAccount>(azureAccountExtensionId);
      if (extension) {
        try {
          // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
          if (
            semver.lt(extension.packageJSON.version, minAccountExtensionVersion)
          ) {
            return "needsUpdate";
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
      registerEvent(
        "azureAccount.onFiltersChanged",
        azureAccount.onFiltersChanged,
        async (context) => {
          context.errorHandling.suppressDisplay = true;
          context.telemetry.suppressIfSuccessful = true;
          await this.refresh(context);
        }
      );
      await commands.executeCommand(
        "setContext",
        "isAzureAccountInstalled",
        true
      );
      return azureAccount;
    } else {
      return "notInstalled";
    }
  }
}
