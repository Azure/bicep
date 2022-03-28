// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { commands, Uri } from "vscode";
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
  BicepDeploymentScopeParams,
  BicepDeploymentScopeResponse,
  BicepDeployParams,
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
    // Handle spaces/special characters in folder names.
    const textDocument = TextDocumentIdentifier.create(
      encodeURIComponent(documentUri.path)
    );
    this.outputChannelManager.appendToOutputChannel(
      `Starting deployment of ${documentPath}`
    );

    context.errorHandling.suppressDisplay = true;

    try {
      const bicepDeploymentScopeParams: BicepDeploymentScopeParams = {
        textDocument,
      };
      const deploymentScopeResponse: BicepDeploymentScopeResponse =
        await this.client.sendRequest("workspace/executeCommand", {
          command: "getDeploymentScope",
          arguments: [bicepDeploymentScopeParams],
        });
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        this.outputChannelManager.appendToOutputChannel(
          "Unable to deploy. Please fix below errors:\n " +
            deploymentScopeResponse?.errorMessage
        );
        return;
      }

      context.telemetry.properties.targetScope = deploymentScope;
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
          await this.handleResourceGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template
          );
          break;
        case "subscription":
          await this.handleSubscriptionDeployment(
            context,
            documentUri,
            deploymentScope,
            template
          );
          break;
        case "managementGroup":
          await this.handleManagementGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template
          );
          break;
        case "tenant": {
          throw new Error(
            "Tenant scope deployment is not currently supported."
          );
        }
        default: {
          throw new Error(
            deploymentScopeResponse?.errorMessage ??
              "Unknown error determining target scope"
          );
        }
      }
    } catch (err) {
      let errorMessage: string;

      if (err instanceof UserCancelledError) {
        errorMessage = `Deployment canceled for ${documentPath}.`;
      }
      // Long-standing issue that is pretty common for all Azure calls, but can be fixed with a simple reload of VS Code.
      // https://github.com/microsoft/vscode-azure-account/issues/53
      else if (parseError(err).message === "Entry not found in cache.") {
        errorMessage = `Deployment canceled for ${documentPath}. Your VS Code window must be reloaded to perform this action.`;
        context.errorHandling.suppressReportIssue = true;
        context.errorHandling.buttons = [
          {
            title: localize("reloadWindow", "Reload Window"),
            callback: async (): Promise<void> => {
              await commands.executeCommand("workbench.action.reloadWindow");
            },
          },
        ];
      } else {
        errorMessage = `Deployment failed for ${documentPath}. ${
          parseError(err).message
        }`;
      }
      this.outputChannelManager.appendToOutputChannel(errorMessage);
      throw err;
    }
  }

  private async handleManagementGroupDeployment(
    context: IActionContext,
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
          documentUri.fsPath,
          parameterFilePath,
          managementGroupId,
          deploymentScope,
          location,
          template,
          managementGroupTreeItem.subscription
        );
      }
    }
  }

  private async handleResourceGroupDeployment(
    context: IActionContext,
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
        documentUri.fsPath,
        parameterFilePath,
        resourceGroupId,
        deploymentScope,
        "",
        template,
        resourceGroupTreeItem.subscription
      );
    }
  }

  private async handleSubscriptionDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string
  ): Promise<void> {
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
      documentUri.fsPath,
      parameterFilePath,
      subscriptionId,
      deploymentScope,
      location,
      template,
      subscription
    );
  }

  private async sendDeployCommand(
    documentPath: string,
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
        documentPath,
        parameterFilePath,
        id,
        deploymentScope,
        location,
        template,
        token,
        expiresOnTimestamp,
      };
      const deploymentResponse: string = await this.client.sendRequest(
        "workspace/executeCommand",
        {
          command: "deploy",
          arguments: [bicepDeployParams],
        }
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
