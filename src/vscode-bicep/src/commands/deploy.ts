// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode from "vscode";
import { ext } from "../extensionVariables";
import { Command } from "./types";
import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";
import {
  IActionContext,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";
import { appendToOutputChannel } from "../utils/logger";
import { LocationTreeItem } from "../deploy/tree/LocationTreeItem";
import { deploymentScopeRequestType } from "../language";
import { AzResourceGroupTreeItem } from "../deploy/tree/AzResourceGroupTreeItem";
import { selectParameterFile } from "../deploy/selectParameterFile";
import { AzLoginTreeItem } from "../deploy/tree/AzLoginTreeItem";

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

    const fileName = path.basename(documentUri.fsPath);
    appendToOutputChannel(`Started deployment of ${fileName}`);

    if (documentUri.scheme === "output") {
      // The output panel in VS Code was implemented as a text editor by accident. Due to breaking change concerns,
      // it won't be fixed in VS Code, so we need to handle it on our side.
      // See https://github.com/microsoft/vscode/issues/58869#issuecomment-422322972 for details.
      vscode.window.showInformationMessage(
        "We are unable to get the Bicep file to build when the output panel is focused. Please focus a text editor first when running the command."
      );

      return;
    }

    try {
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: TextDocumentIdentifier.create(documentUri.fsPath) }
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

      await ext.azLoginTreeItem.showTreeItemPicker<AzLoginTreeItem>(
        "",
        _context
      );

      if (deploymentScope == "resourceGroup") {
        await handleResourceGroupDeployment(
          _context,
          documentUri,
          deploymentScope,
          template,
          this.client
        );
      } else if (deploymentScope == "subscription") {
        await handleSubscriptionDeployment(
          _context,
          documentUri,
          deploymentScope,
          template,
          this.client
        );
      } else if (deploymentScope == "managementGroup") {
        await handleManagementGroupDeployment(
          _context,
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
        documentUri.fsPath,
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
  const parameterFilePath = await selectParameterFile(context, documentUri);

  if (resourceGroupId) {
    await sendDeployCommand(
      documentUri.fsPath,
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
  documentUri: vscode.Uri,
  deploymentScope: string,
  template: string,
  client: LanguageClient
) {
  const locationTreeItem =
    await ext.azLocationTree.showTreeItemPicker<LocationTreeItem>("", context);
  const location = locationTreeItem.label;
  const subscriptionId = locationTreeItem.subscription.subscriptionPath;

  if (location && subscriptionId) {
    const parameterFilePath = await selectParameterFile(context, documentUri);

    await sendDeployCommand(
      documentUri.fsPath,
      parameterFilePath,
      subscriptionId,
      deploymentScope,
      location,
      template,
      client
    );
  }
}

async function sendDeployCommand(
  documentPath: string,
  parameterFilePath: string,
  id: string,
  deploymentScope: string,
  location: string,
  template: string,
  client: LanguageClient
) {
  const deployOutput: string = await client.sendRequest(
    "workspace/executeCommand",
    {
      command: "deploy",
      arguments: [
        documentPath,
        parameterFilePath,
        id,
        deploymentScope,
        location,
        template,
      ],
    }
  );
  appendToOutputChannel(deployOutput);
}
