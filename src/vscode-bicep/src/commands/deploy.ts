// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";
import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";

import {
  AzExtTreeDataProvider,
  IActionContext,
  IAzureQuickPickItem,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";

import {
  BicepDeployParams,
  bicepDeployRequestType,
  deploymentScopeRequestType,
} from "../language";
import { AzLoginTreeItem } from "../tree/AzLoginTreeItem";
import { AzResourceGroupTreeItem } from "../tree/AzResourceGroupTreeItem";
import { LocationTreeItem } from "../tree/LocationTreeItem";
import { TreeManager } from "../tree/TreeManager";
import { localize } from "../utils/localize";
import { OutputChannelManager } from "../utils/OutputChannelManager";
import { Command } from "./types";

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
    _context: IActionContext,
    documentUri?: vscode.Uri | undefined
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

    try {
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: textDocument }
      );
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        this.outputChannelManager.appendToOutputChannel(
          "Deployment failed. " + deploymentScopeResponse?.errorMessage
        );
        return;
      }

      this.outputChannelManager.appendToOutputChannel(
        `Scope specified in ${path.basename(documentPath)} -> ${deploymentScope}`
      );

      // Shows a treeView that allows user to log in to Azure. If the user is already logged in, then does nothing.
      const azLoginTreeItem: AzLoginTreeItem = new AzLoginTreeItem();
      const azExtTreeDataProvider = new AzExtTreeDataProvider(
        azLoginTreeItem,
        ""
      );
      await azExtTreeDataProvider.showTreeItemPicker<AzLoginTreeItem>(
        "",
        _context
      );

      if (deploymentScope == "resourceGroup") {
        await this.handleResourceGroupDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template
        );
      } else if (deploymentScope == "subscription") {
        await this.handleSubscriptionDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template
        );
      } else if (deploymentScope == "managementGroup") {
        await this.handleManagementGroupDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template
        );
      } else if (deploymentScope == "tenant") {
        this.outputChannelManager.appendToOutputChannel(
          "Tenant scope deployment is not currently supported."
        );
      } else {
        this.outputChannelManager.appendToOutputChannel(
          "Deployment failed. " + deploymentScopeResponse?.errorMessage
        );
      }
    } catch (exception) {
      if (exception instanceof UserCancelledError) {
        this.outputChannelManager.appendToOutputChannel(
          "Deployment was canceled."
        );
      } else {
        this.client.error("Deploy failed", parseError(exception).message, true);
      }
    }
  }

  private async handleManagementGroupDeployment(
    context: IActionContext,
    textDocument: TextDocumentIdentifier,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string
  ) {
    const managementGroupTreeItem =
      await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker<LocationTreeItem>(
        "",
        context
      );
    const managementGroupId = managementGroupTreeItem.id;

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
          template
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

      await this.sendDeployCommand(
        textDocument,
        parameterFilePath,
        resourceGroupId,
        deploymentScope,
        "",
        template
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
    const subscriptionId = locationTreeItem.subscription.subscriptionPath;
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
      template
    );
  }

  private async sendDeployCommand(
    textDocument: TextDocumentIdentifier,
    parameterFilePath: string,
    id: string,
    deploymentScope: string,
    location: string,
    template: string
  ) {
    const bicepDeployParams: BicepDeployParams = {
      textDocument,
      parameterFilePath,
      id,
      deploymentScope,
      location,
      template,
    };
    const deploymentResponse: string = await this.client.sendRequest(
      bicepDeployRequestType,
      bicepDeployParams
    );

    this.outputChannelManager.appendToOutputChannel(deploymentResponse);
  }

  private async selectParameterFile(
    _context: IActionContext,
    sourceUri: Uri | undefined
  ): Promise<string> {
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

    this.outputChannelManager.appendToOutputChannel(
      `No parameter file was provided`
    );

    return "";
  }

  private async createParameterFileQuickPickList(): Promise<
    IAzureQuickPickItem[]
  > {

    return [this._none].concat([this._browse]);
  }
}
