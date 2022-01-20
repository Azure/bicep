// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext, parseError } from "vscode-azureextensionui";
import { AzureAccount } from '../azure-account.api';
import { commands, ExtensionContext } from 'vscode';

export class DeployCommand implements Command {
  public readonly id = "bicep.deploy";
  public readonly outputChannel =
    vscode.window.createOutputChannel("Bicep Operations");

  public constructor(private readonly client: LanguageClient) { }

  public async execute(
    _context: IActionContext,
    extensionContext: ExtensionContext,
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
      const azureAccount = vscode.extensions.getExtension<AzureAccount>('ms-vscode.azure-account')!.exports;

      if (!(await azureAccount.waitForLogin())) {
        return commands.executeCommand('azure-account.askForLogin');
      }

    } catch (err) {
      this.client.error("Deploy failed", parseError(err).message, true);
    }
  }
}
