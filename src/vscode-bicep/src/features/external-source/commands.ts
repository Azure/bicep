// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { IActionContext } from "../../infrastructure/action-context";
import { Command, CommandManager } from "../../infrastructure/commands";
import { Disposable } from "../../infrastructure/lifecycle";
import { BicepExternalSourceContentProvider } from "./external-source-content";
import { BicepExternalSourceScheme } from "./external-source-uri";

export class ShowModuleSourceFileCommand implements Command {
  public readonly id = "bicep.internal.showModuleSourceFile";
  public disclaimerShownThisSession = false;

  public async execute(_context: IActionContext, _documentUri: Uri, targetUri: string): Promise<void> {
    const uri = Uri.parse(targetUri, true);
    const doc = await vscode.workspace.openTextDocument(uri);

    await vscode.window.showTextDocument(doc);
  }
}

export async function activateExternalSourceFeature(
  extension: Disposable,
  commandManager: CommandManager,
  languageClient: LanguageClient,
): Promise<void> {
  extension.register(
    vscode.workspace.registerTextDocumentContentProvider(
      BicepExternalSourceScheme,
      new BicepExternalSourceContentProvider(languageClient),
    ),
  );
  await commandManager.registerCommands(new ShowModuleSourceFileCommand());
}
