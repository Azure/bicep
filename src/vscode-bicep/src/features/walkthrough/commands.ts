// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as os from "os";
import path from "path";
import { IActionContext, IAzureQuickPickItem, UserCancelledError } from "@microsoft/vscode-azext-utils";
import * as fse from "fs-extra";
import vscode, { TextDocument, TextEditor, Uri, ViewColumn, window, workspace } from "vscode";
import { Command, CommandManager } from "../../infrastructure/commands";
import { bicepFileExtension } from "../../language/constants";

const paramsCode =
  "param location string = resourceGroup().location\n" +
  "param appPlanName string = '${uniqueString(resourceGroup().id)}plan'\n" +
  "\n";

const resourcesCode = `
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appPlanName
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: '\${appServicePlan.name}storage'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
`;

export class WalkthroughCopyToClipboardCommand implements Command {
  public readonly id = "bicep.gettingStarted.copyToClipboard";

  public async execute(
    context: IActionContext,
    _documentUri: Uri,
    args: { step: "params" | "resources" },
  ): Promise<void> {
    const step = args.step;
    context.telemetry.properties.step = step;

    const code = step === "params" ? paramsCode : resourcesCode;
    await vscode.env.clipboard.writeText(code);
  }
}

export class WalkthroughCreateBicepFileCommand implements Command {
  public static id = "bicep.gettingStarted.createBicepFile";
  public readonly id = WalkthroughCreateBicepFileCommand.id;

  public async execute(): Promise<TextEditor> {
    return await createAndOpenBicepFile("");
  }
}

export class WalkthroughOpenBicepFileCommand implements Command {
  public static id = "bicep.gettingStarted.openBicepFile";
  public readonly id = WalkthroughOpenBicepFileCommand.id;

  public async execute(context: IActionContext): Promise<TextEditor> {
    return await queryAndOpenBicepFile(context);
  }
}

export async function activateWalkthroughFeature(commandManager: CommandManager): Promise<void> {
  await commandManager.registerCommands(
    new WalkthroughCopyToClipboardCommand(),
    new WalkthroughCreateBicepFileCommand(),
    new WalkthroughOpenBicepFileCommand(),
  );
}

async function createAndOpenBicepFile(fileContents: string): Promise<vscode.TextEditor> {
  const folder: Uri =
    (workspace.workspaceFolders ? workspace.workspaceFolders[0].uri : undefined) ?? Uri.file(os.homedir());
  const uri: Uri | undefined = await window.showSaveDialog({
    title: "Save new Bicep file",
    defaultUri: Uri.joinPath(folder, "main"),
    filters: { "Bicep files": [bicepFileExtension] },
  });
  if (!uri) {
    throw new UserCancelledError("saveDialog");
  }

  const filePath = uri.fsPath;
  if (!filePath) {
    throw new Error(`Can't save file to location ${uri.toString()}`);
  }

  await fse.writeFile(filePath, fileContents, { encoding: "utf-8" });

  const document: TextDocument = await workspace.openTextDocument(uri);
  return await vscode.window.showTextDocument(document, vscode.ViewColumn.Beside);
}

async function queryAndOpenBicepFile(context: IActionContext): Promise<TextEditor> {
  const uri: Uri = await queryUserForBicepFile(context);
  const document: TextDocument = await workspace.openTextDocument(uri);
  return await window.showTextDocument(document, ViewColumn.Beside);
}

async function queryUserForBicepFile(context: IActionContext): Promise<Uri> {
  const foundBicepFiles = (await workspace.findFiles("**/*.bicep", undefined)).filter((file) => !!file.fsPath);

  if (foundBicepFiles.length === 0) {
    return await browseForFile(context);
  }

  const entries: IAzureQuickPickItem<Uri | undefined>[] = foundBicepFiles.map((uri) => {
    const workspaceRoot: string | undefined = workspace.getWorkspaceFolder(uri)?.uri.fsPath;
    const relativePath = workspaceRoot ? path.relative(workspaceRoot, uri.fsPath) : path.basename(uri.fsPath);

    return <IAzureQuickPickItem<Uri>>{
      label: relativePath,
      data: uri,
    };
  });
  const browse: IAzureQuickPickItem<Uri | undefined> = {
    label: "Browse...",
    data: undefined,
  };
  entries.unshift(browse);

  const response = await context.ui.showQuickPick(entries, {
    placeHolder: "Select a Bicep file to open",
  });

  if (response === browse) {
    return await browseForFile(context);
  } else if (response.data) {
    return response.data;
  } else {
    throw new Error("Internal error: queryUserForBicepFile: response.data should be truthy");
  }
}

async function browseForFile(context: IActionContext): Promise<Uri> {
  const browsedFile: Uri[] = await context.ui.showOpenDialog({
    title: "Open a Bicep file",
    filters: { "Bicep files": [bicepFileExtension] },
  });

  return browsedFile[0];
}
