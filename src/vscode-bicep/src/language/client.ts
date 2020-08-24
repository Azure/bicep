/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import * as vscode from "vscode";
import * as lsp from "vscode-languageclient/node";
import { existsSync } from "fs";

import { getLogger } from "../uitls/logger";

const dotnetRuntimeVersion = "3.1";
const packagedServerPath = "bicepLanguageServer/Bicep.LangServer.dll";

export async function launchLanugageServiceWithProgressReport(
  context: vscode.ExtensionContext
): Promise<void> {
  await vscode.window.withProgress(
    {
      title: "Launching Bicep language service...",
      location: vscode.ProgressLocation.Window,
    },
    async () => await launchLanguageService(context)
  );
}

async function launchLanguageService(
  context: vscode.ExtensionContext
): Promise<void> {
  getLogger().info("Launching Bicep language service...");

  const dotnetCommandPath = await ensureDotnetRuntimeInstalled();
  getLogger().debug(`Found dotnet command at '${dotnetCommandPath}'.`);

  const languageServerPath = ensureLanguageServerExists(context);
  getLogger().debug(`Found language server at '${languageServerPath}'.`);

  const serverExecutable: lsp.Executable = {
    command: dotnetCommandPath,
    args: [languageServerPath],
  };

  const serverOptions: lsp.ServerOptions = {
    run: serverExecutable,
    debug: serverExecutable,
  };

  const clientOptions: lsp.LanguageClientOptions = {
    documentSelector: [{ language: "bicep" }],
    progressOnInitialization: true,
    synchronize: {
      fileEvents: vscode.workspace.createFileSystemWatcher("**/*.bicep"),
    },
  };

  const client = new lsp.LanguageClient(
    "bicep",
    "Bicep",
    serverOptions,
    clientOptions
  );

  // TODO: expose Trace as a configuration item.
  client.trace = lsp.Trace.Off;
  client.registerProposedFeatures();

  context.subscriptions.push(client.start());

  getLogger().info("Bicep language service started.");
}

async function ensureDotnetRuntimeInstalled(): Promise<string> {
  const result = await vscode.commands.executeCommand<{ dotnetPath: string }>(
    "dotnet.acquire",
    { version: dotnetRuntimeVersion }
  );

  if (!result) {
    throw new Error(`Failed to install .NET runtime v${dotnetRuntimeVersion}.`);
  }

  return result.dotnetPath;
}

function ensureLanguageServerExists(context: vscode.ExtensionContext): string {
  const languageServerPath = process.env.LANGUAGE_SEVER_PATH
    ? context.asAbsolutePath(process.env.LANGUAGE_SEVER_PATH) // Local server for debugging.
    : context.asAbsolutePath(packagedServerPath); // Packaged server.

  if (!existsSync(languageServerPath)) {
    throw new Error(
      `Language server does not exist at '${languageServerPath}'.`
    );
  }

  return languageServerPath;
}
