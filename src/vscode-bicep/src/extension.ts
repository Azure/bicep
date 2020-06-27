/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
// tslint:disable
"use strict";

import * as path from "path";

import { acquireSharedDotnetInstallation } from './acquisition/acquireSharedDotnetInstallation';
import { downloadDotnetVersion, bicepOutputLabel, bicepDebugOutputLabel } from './common/constants';
import { workspace, Disposable, ExtensionContext, window } from "vscode";
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
    // TODO: Unify the path for VSIX package and local debugging
    const serverExe = `${__dirname}/../../Bicep.LangServer/bin/Debug/netcoreapp3.1/Bicep.LangServer.exe`;

    // Create output channel to show extension debug information
    // NOTE(jcotillo) debug info should go to a file as telemetry info
    let info = window.createOutputChannel(bicepDebugOutputLabel);

    try {
        const result = await getDotNetPath();        

        //Write to output.
        info.appendLine(`DotNet version installed: '${result}'`);
    } catch (err) {
        info.appendLine(`Error: ${err}`);
    }

    // If the extension is launched in debug mode then the debug server options are used
    // Otherwise the run options are used
    let serverOptions: ServerOptions = {
        // run: { command: serverExe, args: ['-lsp', '-d'] },
        run: {
            command: serverExe,
            args: []
        },
        // debug: { command: serverExe, args: ['-lsp', '-d'] }
        debug: {
            command: serverExe,
            args: []
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

async function getDotNetPath(): Promise<string | undefined> {
    let dotnetPath: string | undefined;
    return await acquireSharedDotnetInstallation(downloadDotnetVersion);    
}
