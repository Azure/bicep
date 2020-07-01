/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
// tslint:disable
"use strict";

import * as path from "path";
import { existsSync } from "fs";

import { acquireSharedDotnetInstallation } from './acquisition/acquireSharedDotnetInstallation';
import { 
    downloadDotnetVersion,
    bicepOutputLabel,
    bicepDebugOutputLabel,
    languageServerFolderName,
    languageServerName,
    languageServerDllName,
    defaultTraceLevel,
    workspaceSettings
} from './common/constants';
import { workspace, ExtensionContext, window } from "vscode";
import {
    LanguageClient,
    LanguageClientOptions,
    SettingMonitor,
    ServerOptions,
    TransportKind,
    InitializeParams
} from "vscode-languageclient";
import { Trace } from "vscode-jsonrpc";

export async function activate(context: ExtensionContext) {
    // The server is implemented in .net core
    
    // Create output channel to show extension debug information
    // NOTE(jcotillo) debug info should go to a file as telemetry info
    let info = window.createOutputChannel(bicepDebugOutputLabel);

    try {
        const dotNetRuntimePath = await getDotNetRuntimePath();
        //Write to output.
        info.appendLine(`DotNet version installed: '${dotNetRuntimePath}'`);
        const languageServerPath = getLanguageServerPath(context);
        //Write to output.
        info.appendLine(`Bicep language server path: '${languageServerPath}'`);
        startLanguageServer(context, languageServerPath, dotNetRuntimePath);        
    } catch (err) {
        info.appendLine(`Error: ${err}`);
    }
}

async function getDotNetRuntimePath(): Promise<string | undefined> {
    return await acquireSharedDotnetInstallation(downloadDotnetVersion);    
}

function getLanguageServerPath(context: ExtensionContext) {
    const serverFolderPath = context.asAbsolutePath(languageServerFolderName);
    const fullPath = path.join(serverFolderPath, languageServerDllName);
    if (!existsSync(serverFolderPath) || !existsSync(fullPath)) {
        throw new Error(`Cannot find the ${languageServerName} at ${fullPath}. Only template string expression functionality will be available.`);
    }
    return fullPath;
}

function startLanguageServer(context: ExtensionContext, languageServerPath: string, dotNetRuntimePath: string) {    
    let trace: string = workspace.getConfiguration(workspaceSettings.prefix).get<string>(workspaceSettings.traceLevel)
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
                pattern: "**/*.arm",
                language: "bicep",
                scheme: "file"
            },
            {
                language: "bicep",
                scheme: "untitled"
            }
        ],
        progressOnInitialization: true,
        synchronize: {
            // Synchronize the setting section 'bicep' to the server
            configurationSection: "bicep",
            fileEvents: workspace.createFileSystemWatcher("**/*.arm")
        }
    };

    // Create the language client and start the client.
    const client = new LanguageClient(
        "bicep",
        bicepOutputLabel,
        serverOptions,
        clientOptions
    );
    client.registerProposedFeatures();
    client.trace = Trace.Verbose;
    let disposable = client.start();

    // Push the disposable to the context's subscriptions so that the
    // client can be deactivated on extension deactivation
    context.subscriptions.push(disposable);
}
