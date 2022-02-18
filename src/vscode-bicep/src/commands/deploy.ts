// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import vscode, { Uri } from "vscode";
import { ext } from "../extensionVariables";
import { Command } from "./types";
import { LanguageClient, TextDocumentIdentifier } from "vscode-languageclient/node";
import {
  IActionContext,
  IAzureQuickPickItem,
  parseError,
  UserCancelledError,
} from "@microsoft/vscode-azext-utils";
import { AzureAccount } from "../azure-account.api";
import { DefaultAzureCredential } from "@azure/identity";
import {
  ManagementGroupsAPI,
  ManagementGroup,
} from "@azure/arm-managementgroups";
import { SubscriptionClient } from "@azure/arm-subscriptions";
import { appendToOutputChannel } from "../utils/logger";
import { AzureAccountTreeItem } from "../tree/AzureAccountTreeItem";
import { AzTreeItem } from "../tree/AzTreeItem";
import { deploymentScopeRequestType } from "../language";

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
      const path = decodeURIComponent(documentUri.path.substring(1));;
      const deploymentScopeResponse = await this.client.sendRequest(
        deploymentScopeRequestType,
        { textDocument: TextDocumentIdentifier.create(path) }
      );
      const deploymentScope = deploymentScopeResponse?.scope;

      await ext.tree.showTreeItemPicker<AzureAccountTreeItem>("", _context);

      if (deploymentScope == "ResourceGroup") {
        await handleResourceGroupDeployment(
          _context,
          documentUri,
          deploymentScope,
          this.client
        );
      } else if (deploymentScope == "Subscription") {
        await handleSubscriptionDeployment(
          _context,
          documentUri,
          deploymentScope,
          this.client
        );
      } else if (deploymentScope == "ManagementGroup") {
        await handleManagementGroupDeployment(
          _context,
          documentUri,
          deploymentScope,
          this.client
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

async function loadSubscriptionItems() {
  const azureAccount = vscode.extensions.getExtension<AzureAccount>(
    "ms-vscode.azure-account"
  )!.exports;

  const subscriptions = azureAccount.subscriptions;

  subscriptions.sort((a, b) =>
    (a.subscription.displayName || "").localeCompare(
      b.subscription.displayName || ""
    )
  );
  return subscriptions.map((subscription) => ({
    label: subscription.subscription.displayName || "",
    subscription,
  }));
}

async function loadLocationItems(subscriptionId: string) {
  const azureAccount = vscode.extensions.getExtension<AzureAccount>(
    "ms-vscode.azure-account"
  )!.exports;
  const session = azureAccount.sessions[0];
  const subscriptionClient = new SubscriptionClient(session.credentials2);
  const locations = await subscriptionClient.subscriptions.listLocations(
    subscriptionId
  );

  const locationArray = [];
  for await (const location of locations) {
    locationArray.push(location);
  }

  locationArray.sort((a, b) => (a.name || "").localeCompare(b.name || ""));
  return locationArray.map((location) => ({
    label: location.name || "",
    location,
  }));
}

async function handleManagementGroupDeployment(
  context: IActionContext,
  documentUri: vscode.Uri,
  deploymentScope: string,
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
      client
    );
  }
}

async function handleResourceGroupDeployment(
  context: IActionContext,
  documentUri: vscode.Uri,
  deploymentScope: string,
  client: LanguageClient
) {
  const resourceGroupTreeItem = await ext.azTree.showTreeItemPicker<AzTreeItem>(
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
      client
    );
  }
}

async function handleSubscriptionDeployment(
  context: IActionContext,
  documentUri: vscode.Uri,
  deploymentScope: string,
  client: LanguageClient
) {
  const subscription = await getSubscription(context);
  const subscriptionId = subscription?.subscription.subscriptionId;

  if (subscriptionId) {
    const parameterFilePath = await selectParameterFile(context, documentUri);

    const locations = await loadLocationItems(subscriptionId);
    const location = await context.ui.showQuickPick(locations, {
      placeHolder: "Please select location",
    });

    const id = subscription.subscription.id;
    if (location && id) {
      await sendDeployCommand(
        documentUri.fsPath,
        parameterFilePath,
        id,
        deploymentScope,
        location.label,
        client
      );
    }
  }
}

async function sendDeployCommand(
  documentPath: string,
  parameterFilePath: string,
  id: string,
  deploymentScope: string,
  location: string,
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
      ],
    }
  );
  appendToOutputChannel(deployOutput);
}

async function getSubscription(context: IActionContext) {
  const subscriptions = await loadSubscriptionItems();
  const quickPickItem = await context.ui.showQuickPick(subscriptions, {
    placeHolder: "Please select subscription",
  });

  return quickPickItem.subscription;
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
