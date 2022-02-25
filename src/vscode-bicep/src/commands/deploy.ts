// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";
import { ext } from "../extensionVariables";
import { Command } from "./types";
import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";
import {
  IActionContext,
  IAzureQuickPickItem,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";
import { DefaultAzureCredential } from "@azure/identity";
import {
  ManagementGroupsAPI,
  ManagementGroup,
} from "@azure/arm-managementgroups";
import { appendToOutputChannel } from "../utils/logger";
import { LocationTreeItem } from "../deploy/tree/LocationTreeItem";
import { deploymentScopeRequestType } from "../language";
import { AzResourceGroupTreeItem } from "../deploy/tree/AzResourceGroupTreeItem";
import { AzLoginTreeItem } from "../deploy/tree/AzLoginTreeItem";

export class DeployCommand implements Command {
  public readonly id = "bicep.deploy";
  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    _context: IActionContext,
    documentUri?: vscode.Uri | undefined
  ): Promise<void> {
    const document = vscode.window.activeTextEditor?.document;
    documentUri ??= document?.uri;

    if (!documentUri) {
      return;
    }

    const fileName = path.basename(documentUri.fsPath);
    appendToOutputChannel(`Started deployment of "${fileName}"`);

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
      const path = decodeURIComponent(documentUri.path.substring(1));
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: TextDocumentIdentifier.create(path) }
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
        `Scope specified in "${fileName}": "${deploymentScope}"`
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
        appendToOutputChannel("Deployment was cancelled.");
      } else {
        this.client.error("Deploy failed", parseError(exception).message, true);
      }
    }
  }
}

async function loadManagementGroupItems() {
  const managementGroupsAPI = new ManagementGroupsAPI(
    new DefaultAzureCredential()
  );
  const managementGroups = await managementGroupsAPI.managementGroups.list();
  const managementGroupsArray: ManagementGroup[] = [];

  for await (const managementGroup of managementGroups) {
    managementGroupsArray.push(managementGroup);
  }

  managementGroupsArray.sort((a, b) =>
    (a.name || "").localeCompare(b.name || "")
  );
  return managementGroupsArray.map((mg) => ({
    label: mg.name || "",
    mg,
  }));
}

async function handleManagementGroupDeployment(
  context: IActionContext,
  documentUri: vscode.Uri,
  deploymentScope: string,
  template: string,
  client: LanguageClient
) {
  const managementGroupItems = loadManagementGroupItems();
  const managementGroup = await context.ui.showQuickPick(managementGroupItems, {
    placeHolder: "Please select management group",
  });

  const location = await vscode.window.showInputBox({
    placeHolder: "Please enter location",
  });

  const managementGroupId = managementGroup?.mg.id;
  if (managementGroupId && location) {
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

export interface PartialList<T> extends Array<T> {
  nextLink?: string;
}

export async function selectParameterFile(
  _context: IActionContext,
  sourceUri: Uri | undefined
): Promise<string> {
  const quickPickList: IQuickPickList =
    await createParameterFileQuickPickList();
  const result: IAzureQuickPickItem<IPossibleParameterFile | undefined> =
    await _context.ui.showQuickPick(quickPickList.items, {
      canPickMany: false,
      placeHolder: `Select a parameter file`,
      suppressPersistence: true,
    });

  if (result === quickPickList.browse) {
    const paramsPaths: Uri[] | undefined = await vscode.window.showOpenDialog({
      canSelectMany: false,
      defaultUri: sourceUri,
      openLabel: "Select Parameter File",
    });
    if (paramsPaths && paramsPaths.length == 1) {
      const parameterFilePath = paramsPaths[0].fsPath;
      appendToOutputChannel(
        `Parameter file used in deployment: "${path.basename(
          parameterFilePath
        )}"`
      );
      return parameterFilePath;
    }
  }

  appendToOutputChannel(`Parameter file was not provided`);

  return "";
}

async function createParameterFileQuickPickList(): Promise<IQuickPickList> {
  const none: IAzureQuickPickItem<IPossibleParameterFile | undefined> = {
    label: "$(circle-slash) None",
    data: undefined,
  };
  const browse: IAzureQuickPickItem<IPossibleParameterFile | undefined> = {
    label: "$(file-directory) Browse...",
    data: undefined,
  };

  const pickItems: IAzureQuickPickItem<IPossibleParameterFile | undefined>[] = [
    none,
  ].concat([browse]);

  return {
    items: pickItems,
    none,
    browse,
  };
}

interface IQuickPickList {
  items: IAzureQuickPickItem<IPossibleParameterFile | undefined>[];
  none: IAzureQuickPickItem<IPossibleParameterFile | undefined>;
  browse: IAzureQuickPickItem<IPossibleParameterFile | undefined>;
}

interface IPossibleParameterFile {
  uri: Uri;
  friendlyPath: string;
  isCloseNameMatch: boolean;
  fileNotFound?: boolean;
}
