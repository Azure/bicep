/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import * as os from 'os';
import * as fs from 'fs';
import * as path from "path";
import { existsSync } from "fs";

import { acquireSharedDotnetInstallation } from "./acquisition/acquireSharedDotnetInstallation";
import {
  downloadDotnetVersion,
  bicepOutputLanguageServer,
  bicepOutputExtension,
  languageServerFolderName,
  languageServerName,
  languageServerDllName,
  languageServerPath,
  defaultTraceLevel,
  workspaceSettings,
} from "./common/constants";
import { workspace, commands, languages, window, ExtensionContext, OutputChannel, TextDocumentContentProvider, EventEmitter, Uri, ViewColumn } from "vscode";
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
} from "vscode-languageclient/node";
import { Trace } from "vscode-jsonrpc";

async function activatePreviewCommand(context: ExtensionContext, client: LanguageClient, info: OutputChannel) {
  const previewFileName = 'generated.json';
  const availableLanguages = await languages.getLanguages();
  const armTemplateLanguageId = availableLanguages.includes('arm-template') ? 'arm-template' : 'json';

  const bicepPreviewProvider = new class implements TextDocumentContentProvider {
    onDidChangeEmitter = new EventEmitter<Uri>();
    onDidChange = this.onDidChangeEmitter.event;

    provideTextDocumentContent(uri: Uri): string {
      const encodedData = uri.path.substr(0, uri.path.length - previewFileName.length - 1);
      return decodeURIComponent(encodedData);
    }
  }

  context.subscriptions.push(workspace.registerTextDocumentContentProvider('bicep', bicepPreviewProvider));
  context.subscriptions.push(commands.registerTextEditorCommand('bicep.previewtemplate', async (editor, edit) => {
    if (editor.document.languageId != 'bicep') {
      return;
    }

    try {
      // TODO: can remove this tmp file dependency when the
      // language server supports returning command data directly
      const tmpFile = path.join(os.tmpdir(), 'bicep.generated.json');

      await client.sendRequest('workspace/executeCommand', {
        command: 'compile',
        arguments: [
          editor.document.uri.toString(),
          tmpFile,
        ]
      });

      const content = fs.readFileSync(tmpFile, 'utf8');
      const encodedData = encodeURIComponent(content);

      // the final URI segment sets the document title, so set it and remove it before decoding
      let uri = Uri.parse(`bicep:${encodedData}/${previewFileName}`);
      let doc = await workspace.openTextDocument(uri);

      languages.setTextDocumentLanguage(doc, armTemplateLanguageId);
      await window.showTextDocument(doc, { preview: true, viewColumn: ViewColumn.Beside});
    } catch (err) {
      window.showErrorMessage('Generating template preview failed');
      info.appendLine(`Error: ${err}`);
    }
  }));
}

export async function activate(context: ExtensionContext): Promise<void> {
  // The server is implemented in .net core

  // Create output channel to show extension debug information
  // NOTE(jcotillo) debug info should go to a file as telemetry info
  const info = window.createOutputChannel(bicepOutputExtension);

  try {
    const dotNetRuntimePath = await getDotNetRuntimePath();

    if (!dotNetRuntimePath) {
      throw new Error("Unable to download and install .NET Core.");
    }

    //Write to output.
    info.appendLine(`DotNet version installed: '${dotNetRuntimePath}'`);
    const languageServerPath = getLanguageServerPath(context);
    //Write to output.
    info.appendLine(`Bicep language server path: '${languageServerPath}'`);
    const client = startLanguageServer(context, languageServerPath, dotNetRuntimePath);
    await activatePreviewCommand(context, client, info);
  } catch (err) {
    info.appendLine(`Error: ${err}`);
  }
}

async function getDotNetRuntimePath(): Promise<string | undefined> {
  return await acquireSharedDotnetInstallation(downloadDotnetVersion);
}

function getLanguageServerPath(context: ExtensionContext) {
  const serverFolderPath = context.asAbsolutePath(languageServerFolderName);
  let fullPath = process.env[languageServerPath];
  if (!fullPath) {
    fullPath = path.join(serverFolderPath, languageServerDllName);
  }

  if (!existsSync(fullPath)) {
    throw new Error(`Cannot find the ${languageServerName} at ${fullPath}.`);
  }
  return fullPath;
}

function startLanguageServer(
  context: ExtensionContext,
  languageServerPath: string,
  dotNetRuntimePath: string
) {
  const trace: string =
    workspace
      .getConfiguration(workspaceSettings.prefix)
      .get<string>(workspaceSettings.traceLevel) ||
    // tslint:disable-next-line: strict-boolean-expressions
    defaultTraceLevel;

  const commonArgs = [languageServerPath, "--logLevel", trace];

  // If the extension is launched in debug mode then the debug server options are used
  // Otherwise the run options are used
  const serverOptions: ServerOptions = {
    // run: { command: serverExe, args: ['-lsp', '-d'] },
    run: {
      command: dotNetRuntimePath,
      args: commonArgs,
    },
    // debug: { command: serverExe, args: ['-lsp', '-d'] }
    debug: {
      command: dotNetRuntimePath,
      args: commonArgs,
    },
  };

  // Options to control the language client
  const clientOptions: LanguageClientOptions = {
    // Register the server for plain text documents
    documentSelector: [
      {
        pattern: "**/*.bicep",
        language: "bicep",
        scheme: "file",
      },
      {
        language: "bicep",
        scheme: "untitled",
      },
    ],
    progressOnInitialization: true,
    synchronize: {
      // Synchronize the setting section 'bicep' to the server
      configurationSection: "bicep",
      fileEvents: workspace.createFileSystemWatcher("**/*.bicep"),
    },
  };

  // Create the language client and start the client.
  const client = new LanguageClient(
    "bicep",
    bicepOutputLanguageServer,
    serverOptions,
    clientOptions
  );
  client.registerProposedFeatures();
  client.trace = Trace.Off;
  const disposable = client.start();

  // Push the disposable to the context's subscriptions so that the
  // client can be deactivated on extension deactivation
  context.subscriptions.push(disposable);

  return client;
}
