// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "vscode-azureextensionui";
import { AzureAccount, AzureSession } from "../azure-account.api";
import { commands, window } from "vscode";
import {
  SubscriptionClient,
  SubscriptionModels,
} from "@azure/arm-subscriptions";
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
      const azureAccount = vscode.extensions.getExtension<AzureAccount>(
        "ms-vscode.azure-account"
      )!.exports;

      if (!(await azureAccount.waitForLogin())) {
        return commands.executeCommand("azure-account.askForLogin");
      }

      const subscriptionItems = loadSubscriptionItems(azureAccount);
      const subscription = await _context.ui.showQuickPick(subscriptionItems, {
        placeHolder: "Please select subscription",
      });

      if (subscription) {
        const resourceGroupItems = loadResourceGroupItems(subscription);
        const resourceGroup = await _context.ui.showQuickPick(resourceGroupItems, {
          placeHolder: "Please select resource group",
        });

        const deploymentName = await window.showInputBox({
          prompt: "Enter deployment name",
        });

        for (const session of azureAccount.sessions) {
          const credentials = session.credentials2;

          if (credentials) {
            const subscriptionId = subscription.subscription.subscriptionId;
            const resourceGroupName = resourceGroup?.resourceGroup.id;

            if (subscriptionId && resourceGroupName) {
              const deployOutput: string = await this.client.sendRequest(
                "workspace/executeCommand",
                {
                  command: "deploy",
                  arguments: [
                    documentUri.fsPath,
                    subscriptionId,
                    resourceGroupName,
                    deploymentName,
                  ],
                }
              );
              appendToOutputChannel(deployOutput);
            }
          }
        }
      }
    } catch (err) {
      this.client.error("Deploy failed", parseError(err).message, true);
    }
  }
}

async function loadResourceGroupItems(subscriptionItem: SubscriptionItem) {
  const { session, subscription } = subscriptionItem;
  const resources = new ResourceManagementClient(
    session.credentials2,
    subscription.subscriptionId!
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

async function loadSubscriptionItems(api: AzureAccount) {
  await api.waitForFilters();
  const subscriptionItems: SubscriptionItem[] = [];
  for (const session of api.sessions) {
    const credentials = session.credentials2;
    const subscriptionClient = new SubscriptionClient(credentials);
    const subscriptions = await listAll(
      subscriptionClient.subscriptions,
      subscriptionClient.subscriptions.list()
    );
    subscriptionItems.push(
      ...subscriptions.map((subscription) => ({
        label: subscription.displayName || "",
        description: subscription.subscriptionId || "",
        session,
        subscription,
      }))
    );
  }
  subscriptionItems.sort((a, b) => a.label.localeCompare(b.label));
  return subscriptionItems;
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

interface SubscriptionItem {
  label: string;
  description: string;
  session: AzureSession;
  subscription: SubscriptionModels.Subscription;
}
