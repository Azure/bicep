// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import vscode, { Uri, window, workspace } from "vscode";
import { Command } from "./types";
import { LanguageClient } from "vscode-languageclient/node";
import { insertAiResourceRequestType } from "../language";
import { findOrCreateActiveBicepFile } from "./findOrCreateActiveBicepFile";
import { IActionContext } from "@microsoft/vscode-azext-utils";
import { getBicepConfiguration } from "../language/getBicepConfiguration";

export class InsertAiResourceCommand implements Command {
  public readonly id = "bicep.insertAiResource";

  public constructor(private readonly client: LanguageClient) {}

  public async execute(
    context: IActionContext,
    documentUri?: Uri,
  ): Promise<void> {
    documentUri = await findOrCreateActiveBicepFile(
      context,
      documentUri,
      "Choose which Bicep file to insert a resource into",
    );

    const document = await workspace.openTextDocument(documentUri);
    const editor = await window.showTextDocument(document);

    const openAiEndpoint = getBicepConfiguration().get<string>('experimental.openAiEndpoint');
    const openAiKey = getBicepConfiguration().get<string>('experimental.openAiKey');
    if (!openAiEndpoint) {
      throw `Setting 'bicep.experimental.openAiEndpoint' must be configured`;
    }

    const resourceType = await vscode.window.showInputBox({
      prompt: "Enter a resource type",
      value: 'Microsoft.Compute/virtualMachines'
    });

    if (!resourceType) {
      return;
    }

    const apiVersion = await vscode.window.showInputBox({
      prompt: "Enter an api version",
      value: '2023-03-01'
    });

    if (!apiVersion) {
      return;
    }

    const scenario = await vscode.window.showInputBox({
      prompt: "Enter a scenario",
      value: 'Linux VM with 2 data disks'
    });

    if (!scenario) {
      return;
    }

    await this.client.sendNotification(insertAiResourceRequestType, {
      textDocument:
        this.client.code2ProtocolConverter.asTextDocumentIdentifier(document),
      position: this.client.code2ProtocolConverter.asPosition(
        editor.selection.start,
      ),
      openAiEndpoint,
      openAiKey,
      resourceType,
      apiVersion,
      scenario
    });
  }
}
