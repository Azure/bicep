// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";
import { ext } from "../extensionVariables";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import {
  IActionContext,
  IAzureQuickPickItem,
  parseError,
} from "vscode-azureextensionui";
import { AzureAccount } from "../azure-account.api";
import { SubscriptionTreeItem } from "../tree/SubscriptionTreeItem";
import { ResourceManagementClient } from "@azure/arm-resources";
import { appendToOutputChannel } from "../utils/logger";

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

    appendToOutputChannel(
      `Started deployment of "${path.basename(documentUri.fsPath)}"`
    );

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
      const subscription =
        await ext.tree.showTreeItemPicker<SubscriptionTreeItem>(
          SubscriptionTreeItem.contextValue,
          _context
        );

      if (subscription) {
        const subscriptionId = subscription.subscription.subscriptionId;

        const parameterFilePath = await selectParameterFile(
          _context,
          documentUri
        );

        const deploymentScope = await getDeploymentScope(_context);

        if (deploymentScope == "ResourceGroup") {
          await handleResourceGroupDeployment(
            _context,
            subscriptionId,
            documentUri.fsPath,
            parameterFilePath,
            deploymentScope,
            this.client
          );
        } else if (deploymentScope == "Subscription") {
          const deployOutput: string = await this.client.sendRequest(
            "workspace/executeCommand",
            {
              command: "deploy",
              arguments: [
                documentUri.fsPath,
                parameterFilePath,
                subscriptionId,
                "",
                deploymentScope,
              ],
            }
          );
          appendToOutputChannel(deployOutput);
        }
      }
    } catch (err) {
      this.client.error("Deploy failed", parseError(err).message, true);
    }
  }
}

async function handleResourceGroupDeployment(
  context: IActionContext,
  subscriptionId: string,
  documentPath: string,
  parameterFilePath: string,
  deploymentScope: string,
  client: LanguageClient
) {
  const resourceGroupItems = loadResourceGroupItems(subscriptionId);
  const resourceGroup = await context.ui.showQuickPick(resourceGroupItems, {
    placeHolder: "Please select resource group",
  });

  const resourceGroupName = resourceGroup?.resourceGroup.id;

  if (resourceGroupName) {
    const deployOutput: string = await client.sendRequest(
      "workspace/executeCommand",
      {
        command: "deploy",
        arguments: [
          documentPath,
          parameterFilePath,
          "",
          resourceGroupName,
          deploymentScope,
        ],
      }
    );
    appendToOutputChannel(deployOutput);
  }
}

async function getDeploymentScope(context: IActionContext) {
  const deploymentScopes: IAzureQuickPickItem<string | undefined>[] =
    await createScopesQuickPickList();

  const deploymentScope: IAzureQuickPickItem<string | undefined> =
    await context.ui.showQuickPick(deploymentScopes, {
      canPickMany: false,
      placeHolder: `Select a deployment scope`,
      suppressPersistence: true,
    });

  return deploymentScope.label;
}

async function loadResourceGroupItems(subscriptionId: string) {
  const azureAccount = vscode.extensions.getExtension<AzureAccount>(
    "ms-vscode.azure-account"
  )!.exports;
  const session = azureAccount.sessions[0];

  const resources = new ResourceManagementClient(
    session.credentials2,
    subscriptionId
  );
  const resourceGroups = await listAll(
    resources.resourceGroups,
    resources.resourceGroups.list()
  );
  resourceGroups.sort((a, b) => (a.name || "").localeCompare(b.name || ""));
  return resourceGroups.map((resourceGroup) => ({
    label: resourceGroup.name || "",
    description: resourceGroup.location,
    resourceGroup,
  }));
}

async function listAll<T>(
  client: { listNext(nextPageLink: string): Promise<PartialList<T>> },
  first: Promise<PartialList<T>>
): Promise<T[]> {
  const all: T[] = [];
  for (
    let list = await first;
    list.length || list.nextLink;
    list = list.nextLink ? await client.listNext(list.nextLink) : []
  ) {
    all.push(...list);
  }
  return all;
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

async function createScopesQuickPickList(): Promise<
  IAzureQuickPickItem<string | undefined>[]
> {
  const managementGroup: IAzureQuickPickItem<string | undefined> = {
    label: "ManagementGroup",
    data: undefined,
  };
  const resourceGroup: IAzureQuickPickItem<string | undefined> = {
    label: "ResourceGroup",
    data: undefined,
  };
  const subscription: IAzureQuickPickItem<string | undefined> = {
    label: "Subscription",
    data: undefined,
  };

  const scopes: IAzureQuickPickItem<string | undefined>[] = [managementGroup]
    .concat([resourceGroup])
    .concat([subscription]);

  return scopes;
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
