// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";
import { AccessToken } from "@azure/identity";
import { AzLoginTreeItem } from "../tree/AzLoginTreeItem";
import { AzManagementGroupTreeItem } from "../tree/AzManagementGroupTreeItem";
import { AzResourceGroupTreeItem } from "../tree/AzResourceGroupTreeItem";
import { Command } from "./types";
import { localize } from "../utils/localize";
import { LocationTreeItem } from "../tree/LocationTreeItem";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { TreeManager } from "../tree/TreeManager";

import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";

import {
  AzExtTreeDataProvider,
  IActionContext,
  IAzureQuickPickItem,
  ISubscriptionContext,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";

import {
  BicepDeployParams,
  bicepDeployRequestType,
  deploymentScopeRequestType,
} from "../language";

export class DeployCommand implements Command {
  private _none: IAzureQuickPickItem = {
    label: localize("none", "$(circle-slash) None"),
    data: undefined,
  };
  private _browse: IAzureQuickPickItem = {
    label: localize("browse", "$(file-directory) Browse..."),
    data: undefined,
  };

  public readonly id = "bicep.deploy";

  public constructor(
    private readonly client: LanguageClient,
    private readonly outputChannelManager: OutputChannelManager,
    private readonly treeManager: TreeManager
  ) {}

  public async execute(
    context: IActionContext,
    documentUri: vscode.Uri | undefined
  ): Promise<void> {
    documentUri ??= vscode.window.activeTextEditor?.document.uri;

    if (!documentUri) {
      return;
    }

    if (documentUri.scheme === "output") {
      // The output panel in VS Code was implemented as a text editor by accident. Due to breaking change concerns,
      // it won't be fixed in VS Code, so we need to handle it on our side.
      // See https://github.com/microsoft/vscode/issues/58869#issuecomment-422322972 for details.
      vscode.window.showInformationMessage(
        "Unable to locate an active Bicep file, as the output panel is focused. Please focus a text editor first before running the command."
      );

      return;
    }

    const documentPath = documentUri.fsPath;
    const textDocument = TextDocumentIdentifier.create(documentUri.fsPath);
    this.outputChannelManager.appendToOutputChannel(
      `Started deployment of ${documentPath}`
    );

    context.errorHandling.suppressDisplay = true;

    try {
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: textDocument }
      );
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        this.outputChannelManager.appendToOutputChannel(
          "Unable to deploy. Please fix below errors:\n " +
            deploymentScopeResponse?.errorMessage
        );
        return;
      }

      this.outputChannelManager.appendToOutputChannel(
        `Scope specified in ${path.basename(
          documentPath
        )} -> ${deploymentScope}`
      );

      // Shows a treeView that allows user to log in to Azure. If the user is already logged in, then does nothing.
      const azLoginTreeItem: AzLoginTreeItem = new AzLoginTreeItem();
      const azExtTreeDataProvider = new AzExtTreeDataProvider(
        azLoginTreeItem,
        ""
      );
      await azExtTreeDataProvider.showTreeItemPicker<AzLoginTreeItem>(
        "",
        context
      );

      switch (deploymentScope) {
        case "resourceGroup":
          return await this.handleResourceGroupDeployment(
            context,
            textDocument,
            documentUri,
            deploymentScope,
            template
          );
        case "subscription":
          return await this.handleSubscriptionDeployment(
            context,
            textDocument,
            documentUri,
            deploymentScope,
            template
          );
        case "managementGroup":
          return await this.handleManagementGroupDeployment(
            context,
            textDocument,
            documentUri,
            deploymentScope,
            template
          );
        case "tenant": {
          const tenantScopeNotSupportedMessage =
            "Tenant scope deployment is not currently supported.";
          this.outputChannelManager.appendToOutputChannel(
            tenantScopeNotSupportedMessage
          );
          throw new Error(tenantScopeNotSupportedMessage);
        }
        default: {
          const deploymentFailedMessage =
            "Deployment failed. " + deploymentScopeResponse?.errorMessage;

          this.outputChannelManager.appendToOutputChannel(
            deploymentFailedMessage
          );

          throw new Error(deploymentFailedMessage);
        }
      }
    } catch (exception) {
      if (exception instanceof UserCancelledError) {
        this.outputChannelManager.appendToOutputChannel(
          "Deployment was canceled."
        );
      }

      throw exception;
    }
  }

  private async handleManagementGroupDeployment(
    context: IActionContext,
    textDocument: TextDocumentIdentifier,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string
  ) {
    let managementGroupTreeItem: AzManagementGroupTreeItem | undefined;
    try {
      managementGroupTreeItem =
        await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker<AzManagementGroupTreeItem>(
          "",
          context
        );
    } catch (exception) {
      this.outputChannelManager.appendToOutputChannel(
        "Deployment failed. " + parseError(exception).message
      );

      throw exception;
    }
    const managementGroupId = managementGroupTreeItem?.id;
    const subscription = managementGroupTreeItem.subscription;

    if (managementGroupId) {
      const location = await vscode.window.showInputBox({
        placeHolder: "Please enter location",
      });

      if (location) {
        const parameterFilePath = await this.selectParameterFile(
          context,
          documentUri
        );

        await this.sendDeployCommand(
          textDocument,
          parameterFilePath,
          managementGroupId,
          deploymentScope,
          location,
          template,
          subscription
        );
      }
    }
  }

  private async handleResourceGroupDeployment(
    context: IActionContext,
    textDocument: TextDocumentIdentifier,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string
  ) {
    const resourceGroupTreeItem =
      await this.treeManager.azResourceGroupTreeItem.showTreeItemPicker<AzResourceGroupTreeItem>(
        "",
        context
      );
    const resourceGroupId = resourceGroupTreeItem.id;

    if (resourceGroupId) {
      const parameterFilePath = await this.selectParameterFile(
        context,
        documentUri
      );

      const subscription = resourceGroupTreeItem.subscription;

      await this.sendDeployCommand(
        textDocument,
        parameterFilePath,
        resourceGroupId,
        deploymentScope,
        "",
        template,
        subscription
      );
    }
  }

  private async handleSubscriptionDeployment(
    context: IActionContext,
    textDocument: TextDocumentIdentifier,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string
  ) {
    const locationTreeItem =
      await this.treeManager.azLocationTree.showTreeItemPicker<LocationTreeItem>(
        "",
        context
      );
    const location = locationTreeItem.label;
    const subscription = locationTreeItem.subscription;
    const subscriptionId = subscription.subscriptionPath;

    const parameterFilePath = await this.selectParameterFile(
      context,
      documentUri
    );

    await this.sendDeployCommand(
      textDocument,
      parameterFilePath,
      subscriptionId,
      deploymentScope,
      location,
      template,
      subscription
    );
  }

  private async sendDeployCommand(
    textDocument: TextDocumentIdentifier,
    parameterFilePath: string | undefined,
    id: string,
    deploymentScope: string,
    location: string,
    template: string,
    subscription: ISubscriptionContext
  ) {
    if (!parameterFilePath) {
      this.outputChannelManager.appendToOutputChannel(
        `No parameter file was provided`
      );
      parameterFilePath = "";
    }

    const accessToken: AccessToken = await subscription.credentials.getToken(
      []
    );

    if (accessToken) {
      const token = accessToken.token;
      const expiresOnTimestamp = String(accessToken.expiresOnTimestamp);

      const bicepDeployParams: BicepDeployParams = {
        textDocument,
        parameterFilePath,
        id,
        deploymentScope,
        location,
        template,
        token,
        expiresOnTimestamp,
      };
      const deploymentResponse: string = await this.client.sendRequest(
        bicepDeployRequestType,
        bicepDeployParams
      );
      this.outputChannelManager.appendToOutputChannel(deploymentResponse);
    }
  }

  private async selectParameterFile(
    _context: IActionContext,
    sourceUri: Uri | undefined
  ): Promise<string | undefined> {
    const quickPickItems: IAzureQuickPickItem[] =
      await this.createParameterFileQuickPickList();
    const result: IAzureQuickPickItem = await _context.ui.showQuickPick(
      quickPickItems,
      {
        canPickMany: false,
        placeHolder: `Select a parameter file`,
        suppressPersistence: true,
      }
    );

    if (result == this._browse) {
      const paramsPaths: Uri[] | undefined = await vscode.window.showOpenDialog(
        {
          canSelectMany: false,
          defaultUri: sourceUri,
          openLabel: "Select Parameter File",
          filters: { "JSON Files": ["json", "jsonc"] },
        }
      );
      if (paramsPaths && paramsPaths.length == 1) {
        const parameterFilePath = paramsPaths[0].fsPath;
        this.outputChannelManager.appendToOutputChannel(
          `Parameter file used in deployment -> ${parameterFilePath}`
        );
        return parameterFilePath;
      }
    }

    return undefined;
  }

  private async createParameterFileQuickPickList(): Promise<
    IAzureQuickPickItem[]
  > {
    return [this._none].concat([this._browse]);
  }
}
