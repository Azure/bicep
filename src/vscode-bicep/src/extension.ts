/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
// tslint:disable
"use strict";

import * as os from 'os';
import * as fs from 'fs';
import * as path from "path";
import { existsSync } from "fs";

import { acquireSharedDotnetInstallation } from './acquisition/acquireSharedDotnetInstallation';
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
    bicepLanguageId,
} from './common/constants';
import * as vscode from "vscode";
import {
    LanguageClient,
    LanguageClientOptions,
    ServerOptions,
} from "vscode-languageclient/node";
import { Trace } from "vscode-jsonrpc";

const previewFileName = 'generated.json';

async function activatePreviewCommand(context: vscode.ExtensionContext, client: LanguageClient, info: vscode.OutputChannel) {
    const availableLanguages = await vscode.languages.getLanguages();
    const armTemplateLanguageId = availableLanguages.includes('arm-template') ? 'arm-template' : 'json';

    const bicepPreviewProvider = new class implements vscode.TextDocumentContentProvider {
        onDidChangeEmitter = new vscode.EventEmitter<vscode.Uri>();
        onDidChange = this.onDidChangeEmitter.event;

        provideTextDocumentContent(uri: vscode.Uri): string {
            const encodedData = uri.path.substr(0, uri.path.length - previewFileName.length - 1);
            return decodeURIComponent(encodedData);
        }
    }

    context.subscriptions.push(vscode.workspace.registerTextDocumentContentProvider(bicepLanguageId, bicepPreviewProvider));
    context.subscriptions.push(vscode.commands.registerTextEditorCommand('bicep.previewtemplate', async (editor, edit) => {
        if (editor.document.languageId != bicepLanguageId) {
            return;
        }

        try {
            // TODO: can remove this tmp file dependency when the
            // language server supports returning command data directly
            const tmpFile = path.join(os.tmpdir(), 'bicep.generated.json');

            await client.sendRequest('workspace/executeCommand', {
                command: 'compile',
                arguments: [
                    editor.document.getText(),
                    tmpFile,
                ]
            });

            const content = fs.readFileSync(tmpFile, 'utf8');
            const encodedData = encodeURIComponent(content);

            // the final URI segment sets the document title, so set it and remove it before decoding
            let uri = vscode.Uri.parse(`bicep:${encodedData}/${previewFileName}`);
            let doc = await vscode.workspace.openTextDocument(uri);
            
            vscode.languages.setTextDocumentLanguage(doc, armTemplateLanguageId);
            await vscode.window.showTextDocument(doc, { preview: true, viewColumn: vscode.ViewColumn.Beside});
        } catch (err) {
            vscode.window.showErrorMessage('Generating template preview failed');
            info.appendLine(`Error: ${err}`);
        }
    }));
}

export async function activate(context: vscode.ExtensionContext) {
    // Create output channel to show extension debug information
    // NOTE(jcotillo) debug info should go to a file as telemetry info
    const info = vscode.window.createOutputChannel(bicepOutputExtension);

    try {
        const dotNetRuntimePath = await getDotNetRuntimePath();
        //Write to output.
        info.appendLine(`DotNet version installed: '${dotNetRuntimePath}'`);

        const languageServerPath = getLanguageServerPath(context);
        //Write to output.
        info.appendLine(`Bicep language server path: '${languageServerPath}'`);

        const client = startLanguageServer(context, info, languageServerPath, dotNetRuntimePath);

        await activatePreviewCommand(context, client, info);
    } catch (err) {
        vscode.window.showErrorMessage('Failed to initialize Bicep language server');
        info.appendLine(`Activating extension failed: ${err}`);
    }
}

async function getDotNetRuntimePath(): Promise<string | undefined> {
    return await acquireSharedDotnetInstallation(downloadDotnetVersion);    
}

function getLanguageServerPath(context: vscode.ExtensionContext) {    
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

function startLanguageServer(context: vscode.ExtensionContext, info: vscode.OutputChannel, languageServerPath: string, dotNetRuntimePath: string): LanguageClient {
    let trace: string = vscode.workspace.getConfiguration(workspaceSettings.prefix).get<string>(workspaceSettings.traceLevel)
            // tslint:disable-next-line: strict-boolean-expressions
            || defaultTraceLevel;

    let commonArgs = [
        languageServerPath,
        '--logLevel',
        trace
    ];
    
    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        // run: { command: serverExe, args: ['-lsp', '-d'] },
        run: {
            command: dotNetRuntimePath,
            args: commonArgs
        },
        // debug: { command: serverExe, args: ['-lsp', '-d'] }
        debug: {
            command: dotNetRuntimePath,
            args: commonArgs
        }
    };

    // Options to control the language client
    let clientOptions: LanguageClientOptions = {
        // Register the server for plain text documents
        documentSelector: [
            {
                pattern: "**/*.bicep",
                language: bicepLanguageId,
                scheme: "file"
            },
            {
                language: bicepLanguageId,
                scheme: "untitled"
            }
        ],
        progressOnInitialization: true,
        synchronize: {
            // Synchronize the setting section 'bicep' to the server
            configurationSection: bicepLanguageId,
            fileEvents: vscode.workspace.createFileSystemWatcher("**/*.bicep")
        },
        outputChannel: info,
    };

    // Create the language client and start the client.
    const client = new LanguageClient(
        bicepLanguageId,
        bicepOutputLanguageServer,
        serverOptions,
        clientOptions
    );
    client.registerProposedFeatures();
    client.trace = Trace.Off;
    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);

    return client;
}