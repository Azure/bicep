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
} from "@microsoft/vscode-azext-utils";
import {
  BicepDeploymentScopeParams,
  BicepDeploymentScopeResponse,
  BicepDeploymentWaitForCompletionParams,
  BicepDeploymentStartParams,
  BicepDeploymentStartResponse,
  BicepDeploymentParams,
} from "../language";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";

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
    const deployId = Math.random().toString();
    context.telemetry.properties.deployId = deployId;

    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to deploy"
    );

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

      let deploymentStartResponse: BicepDeploymentStartResponse | undefined;

      switch (deploymentScope) {
        case "resourceGroup":
          deploymentStartResponse = await this.handleResourceGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
          );
          break;
        case "subscription":
          deploymentStartResponse = await this.handleSubscriptionDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
          );
          break;
        case "managementGroup":
          deploymentStartResponse = await this.handleManagementGroupDeployment(
            context,
            documentUri,
            deploymentScope,
            template,
            deployId
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

      await this.sendDeployWaitForCompletionCommand(
        deployId,
        deploymentStartResponse,
        documentPath
      );
    } catch (err) {
      let errorMessage: string;

      if (parseError(err).isUserCancelledError) {
        errorMessage = `Deployment canceled for ${documentPath}.`;
      }
      // Long-standing issue that is pretty common for all Azure calls, but can be fixed with a simple reload of VS Code.
      // https://github.com/microsoft/vscode-azure-account/issues/53
      else if (parseError(err).message === "Entry not found in cache.") {
        errorMessage = `Deployment failed for ${documentPath}. Token cache is out of date. Please reload VS Code and try again. If this problem persists, consider changing the VS Code setting "Azure: Authentication Library" to "MSAL".`;
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
    template: string,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
    const managementGroupTreeItem =
      await this.treeManager.azManagementGroupTreeItem.showTreeItemPicker<AzManagementGroupTreeItem>(
        "",
        context
      );
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

        const [updateParameterFile, updatedParamsWithValues] =
          await this.handleMissingAndDefaultParams(
            context,
            documentUri,
            parameterFilePath
          );

        return await this.sendDeployStartCommand(
          context,
          documentUri.fsPath,
          parameterFilePath,
          managementGroupId,
          deploymentScope,
          location,
          template,
          managementGroupTreeItem.subscription,
          deployId,
          updateParameterFile,
          updatedParamsWithValues
        );
      }
    }

    return undefined;
  }

  private async handleResourceGroupDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
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

      const [updateParameterFile, updatedParamsWithValues] =
        await this.handleMissingAndDefaultParams(
          context,
          documentUri,
          parameterFilePath
        );

      return await this.sendDeployStartCommand(
        context,
        documentUri.fsPath,
        parameterFilePath,
        resourceGroupId,
        deploymentScope,
        "",
        template,
        resourceGroupTreeItem.subscription,
        deployId,
        updateParameterFile,
        updatedParamsWithValues
      );
    }

    return undefined;
  }

  private async handleSubscriptionDeployment(
    context: IActionContext,
    documentUri: vscode.Uri,
    deploymentScope: string,
    template: string,
    deployId: string
  ): Promise<BicepDeploymentStartResponse | undefined> {
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

    const [updateParameterFile, updatedParamsWithValues] =
      await this.handleMissingAndDefaultParams(
        context,
        documentUri,
        parameterFilePath
      );

    return await this.sendDeployStartCommand(
      context,
      documentUri.fsPath,
      parameterFilePath,
      subscriptionId,
      deploymentScope,
      location,
      template,
      subscription,
      deployId,
      updateParameterFile,
      updatedParamsWithValues
    );
  }

  private async sendDeployStartCommand(
    context: IActionContext,
    documentPath: string,
    parameterFilePath: string | undefined,
    id: string,
    deploymentScope: string,
    location: string,
    template: string,
    subscription: ISubscriptionContext,
    deployId: string,
    updateParameterFile: boolean,
    deploymentParams: BicepDeploymentParams[]
  ): Promise<BicepDeploymentStartResponse | undefined> {
    if (!parameterFilePath) {
      context.telemetry.properties.parameterFileProvided = "false";
      this.outputChannelManager.appendToOutputChannel(
        `No parameter file was provided`
      );
      parameterFilePath = "";
    } else {
      context.telemetry.properties.parameterFileProvided = "true";
    }

    const accessToken: AccessToken = await subscription.credentials.getToken(
      []
    );

    if (accessToken) {
      const token = accessToken.token;
      const expiresOnTimestamp = String(accessToken.expiresOnTimestamp);
      const portalUrl = subscription.environment.portalUrl;

      const deploymentStartParams: BicepDeploymentStartParams = {
        documentPath,
        parameterFilePath,
        id,
        deploymentScope,
        location,
        template,
        token,
        expiresOnTimestamp,
        deployId,
        portalUrl,
        updateParameterFile,
        deploymentParams,
      };
      const deploymentStartResponse: BicepDeploymentStartResponse =
        await this.client.sendRequest("workspace/executeCommand", {
          command: "deploy/start",
          arguments: [deploymentStartParams],
        });

      return deploymentStartResponse;
    }

    return undefined;
  }

  private async sendDeployWaitForCompletionCommand(
    deployId: string,
    deploymentStartResponse: BicepDeploymentStartResponse | undefined,
    documentPath: string
  ) {
    if (deploymentStartResponse) {
      this.outputChannelManager.appendToOutputChannel(
        deploymentStartResponse.outputMessage
      );

      if (deploymentStartResponse.isSuccess) {
        const viewDeploymentInPortalMessage =
          deploymentStartResponse.viewDeploymentInPortalMessage;

        if (viewDeploymentInPortalMessage != null) {
          this.outputChannelManager.appendToOutputChannel(
            viewDeploymentInPortalMessage
          );
        }
        const bicepDeploymentWaitForCompletionParams: BicepDeploymentWaitForCompletionParams =
          {
            deployId,
            documentPath,
          };
        const outputMessage: string = await this.client.sendRequest(
          "workspace/executeCommand",
          {
            command: "deploy/waitForCompletion",
            arguments: [bicepDeploymentWaitForCompletionParams],
          }
        );

        this.outputChannelManager.appendToOutputChannel(outputMessage);
      }
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

    let parameterFilePath: string | undefined;

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
        parameterFilePath = paramsPaths[0].fsPath;
        this.outputChannelManager.appendToOutputChannel(
          `Parameter file used in deployment -> ${parameterFilePath}`
        );
      }
    }

    return parameterFilePath;
  }

  private async handleMissingAndDefaultParams(
    _context: IActionContext,
    sourceUri: Uri | undefined,
    parameterFilePath: string | undefined
  ): Promise<[boolean, BicepDeploymentParams[]]> {
    const deployParams: BicepDeploymentParams[] = await this.client.sendRequest(
      "workspace/executeCommand",
      {
        command: "getParametersInfo",
        arguments: [sourceUri?.fsPath, parameterFilePath],
      }
    );

    const updatedParamsWithValues: BicepDeploymentParams[] = [];
    let showUpdateParametersFileInformationMessage = false;
    let updateParametersFile = false;

    for (const deployParam of deployParams) {
      const paramName = deployParam.name;
      if (deployParam.isMissingParam) {
        showUpdateParametersFileInformationMessage = true;
        const missingParamValue = await vscode.window.showInputBox({
          placeHolder: "Please enter value for param: " + paramName,
        });

        const updatedParam: BicepDeploymentParams = {
          name: paramName,
          value: missingParamValue,
          isMissingParam: true,
          displayActualDefault: deployParam.displayActualDefault,
        };
        updatedParamsWithValues.push(updatedParam);
      } else {
        let options: vscode.InputBoxOptions;
        if (deployParam.displayActualDefault) {
          options = {
            prompt: "Press enter to select the default value",
            value: deployParam.value,
            placeHolder: "Please enter value for param: " + paramName,
          };
        } else {
          options = {
            prompt: "Press enter to select the default value",
            value: deployParam.value,
            placeHolder: "Please enter value for param: " + paramName,
          };
        }
        const defaultParamValue = await vscode.window.showInputBox(options);
        const updatedParam: BicepDeploymentParams = {
          name: paramName,
          value: defaultParamValue,
          isMissingParam: true,
          displayActualDefault: deployParam.displayActualDefault,
        };
        updatedParamsWithValues.push(updatedParam);
      }

      if (showUpdateParametersFileInformationMessage) {
        vscode.window
          .showInformationMessage("Do you want to do this?", "Yes", "No")
          .then((answer) => {
            if (answer === "Yes") {
              updateParametersFile = true;
            }
          });
      }
    }

    return [updateParametersFile, updatedParamsWithValues];
  }

  private async createParameterFileQuickPickList(): Promise<
    IAzureQuickPickItem[]
  > {
    return [this._none].concat([this._browse]);
  }
}
