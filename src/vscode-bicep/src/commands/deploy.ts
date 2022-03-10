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

import { AzLoginTreeItem } from "../deploy/tree/AzLoginTreeItem";
import { AzResourceGroupTreeItem } from "../deploy/tree/AzResourceGroupTreeItem";
import { LocationTreeItem } from "../deploy/tree/LocationTreeItem";
import { ext } from "../extensionVariables";
import {
  BicepDeployParams,
  bicepDeployRequestType,
  deploymentScopeRequestType,
} from "../language";
import { appendToOutputChannel } from "../utils/appendToOutputChannel";
import { Command } from "./types";

export class DeployCommand implements Command {
  public readonly id = "bicep.deploy";
  public constructor(private readonly client: LanguageClient) {}

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
    const fileName = path.basename(documentPath);
    appendToOutputChannel(`Started deployment of ${fileName}`);

    try {
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: textDocument }
      );
      const deploymentScope = deploymentScopeResponse?.scope;
      const template = deploymentScopeResponse?.template;

      if (!template) {
        appendToOutputChannel(
          "Deployment failed. " + deploymentScopeResponse?.errorMessage
        );
        return;
      }

      appendToOutputChannel(
        `Scope specified in ${fileName} -> ${deploymentScope}`
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
        await handleResourceGroupDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template,
          this.client
        );
      } else if (deploymentScope == "subscription") {
        await handleSubscriptionDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template,
          this.client
        );
      } else if (deploymentScope == "managementGroup") {
        await handleManagementGroupDeployment(
          _context,
          textDocument,
          documentUri,
          deploymentScope,
          template,
          this.client
        );
      } else if (deploymentScope == "tenant") {
        appendToOutputChannel("Tenant scope deployment is not supported.");
      } else {
        appendToOutputChannel(
          "Deployment failed. " + deploymentScopeResponse?.errorMessage
        );
      }
    } catch (exception) {
      if (exception instanceof UserCancelledError) {
        appendToOutputChannel("Deployment was canceled.");
      } else {
        this.client.error("Deploy failed", parseError(exception).message, true);
      }
    }
  }
}

async function handleManagementGroupDeployment(
  context: IActionContext,
  textDocument: TextDocumentIdentifier,
  documentUri: vscode.Uri,
  deploymentScope: string,
  template: string,
  client: LanguageClient
) {
  const managementGroupTreeItem =
    await ext.azManagementGroupTreeItem.showTreeItemPicker<LocationTreeItem>(
      "",
      context
    );
  const managementGroupId = managementGroupTreeItem.id;

  if (managementGroupId) {
    const location = await vscode.window.showInputBox({
      placeHolder: "Please enter location",
    });

    if (location) {
      const parameterFilePath = await selectParameterFile(context, documentUri);

      await sendDeployCommand(
        textDocument,
        parameterFilePath,
        managementGroupId,
        deploymentScope,
        location,
        template,
        client
      );
    }
  }
}

async function handleResourceGroupDeployment(
  context: IActionContext,
  textDocument: TextDocumentIdentifier,
  documentUri: vscode.Uri,
  deploymentScope: string,
  template: string,
  client: LanguageClient
) {
  const resourceGroupTreeItem =
    await ext.azResourceGroupTreeItem.showTreeItemPicker<AzResourceGroupTreeItem>(
      "",
      context
    );
  const resourceGroupId = resourceGroupTreeItem.id;

  if (resourceGroupId) {
    const parameterFilePath = await selectParameterFile(context, documentUri);

    await sendDeployCommand(
      textDocument,
      parameterFilePath,
      resourceGroupId,
      deploymentScope,
      "",
      template,
      client
    );
  }
}

async function handleSubscriptionDeployment(
  context: IActionContext,
  textDocument: TextDocumentIdentifier,
  documentUri: vscode.Uri,
  deploymentScope: string,
  template: string,
  client: LanguageClient
) {
  const locationTreeItem =
    await ext.azLocationTree.showTreeItemPicker<LocationTreeItem>("", context);
  const location = locationTreeItem.label;
  const subscriptionId = locationTreeItem.subscription.subscriptionPath;
  const parameterFilePath = await selectParameterFile(context, documentUri);

  await sendDeployCommand(
    textDocument,
    parameterFilePath,
    subscriptionId,
    deploymentScope,
    location,
    template,
    client
  );
}

async function sendDeployCommand(
  textDocument: TextDocumentIdentifier,
  parameterFilePath: string,
  id: string,
  deploymentScope: string,
  location: string,
  template: string,
  client: LanguageClient
) {
  const bicepDeployParams: BicepDeployParams = {
    textDocument,
    parameterFilePath,
    id,
    deploymentScope,
    location,
    template,
  };
  const deploymentResponse: string = await client.sendRequest(
    bicepDeployRequestType,
    bicepDeployParams
  );

  appendToOutputChannel(deploymentResponse);
}

async function selectParameterFile(
  _context: IActionContext,
  sourceUri: Uri | undefined
): Promise<string> {
  const quickPickItems: IAzureQuickPickItem[] =
    await createParameterFileQuickPickList();
  const result: IAzureQuickPickItem = await _context.ui.showQuickPick(
    quickPickItems,
    {
      canPickMany: false,
      placeHolder: `Select a parameter file`,
      suppressPersistence: true,
    }
  );

  if (result.label.includes("Browse...")) {
    const paramsPaths: Uri[] | undefined = await vscode.window.showOpenDialog({
      canSelectMany: false,
      defaultUri: sourceUri,
      openLabel: "Select Parameter File",
    });
    if (paramsPaths && paramsPaths.length == 1) {
      const parameterFilePath = paramsPaths[0].fsPath;
      appendToOutputChannel(
        `Parameter file used in deployment -> ${path.basename(
          parameterFilePath
        )}`
      );
      return parameterFilePath;
    }
  }

  appendToOutputChannel(`Parameter file was not provided`);

  return "";
}

async function createParameterFileQuickPickList(): Promise<
  IAzureQuickPickItem[]
> {
  const none: IAzureQuickPickItem = {
    label: "$(circle-slash) None",
    data: undefined,
  };
  const browse: IAzureQuickPickItem = {
    label: "$(file-directory) Browse...",
    data: undefined,
  };

  return [none].concat([browse]);
}
