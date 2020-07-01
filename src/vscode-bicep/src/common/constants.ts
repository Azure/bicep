/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.md in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

// The dotnet version to download and run the language server against
export const downloadDotnetVersion = '3.1';

// The main output label
export const bicepOutputLabel = "Bicep Language Server";

// The extension output label
export const bicepDebugOutputLabel = "Bicep Extension";

// The language server folder name
export const languageServerFolderName = 'languageServer';

// The language server name
export const languageServerName = 'Bicep Language Server';

// The language server binary
export const languageServerDllName = 'Bicep.LangServer.dll';

// The default tracing level
export const defaultTraceLevel = 'Warning';

// The workspace (user settings)
export namespace workspaceSettings {
    export const prefix = 'bicepLanguageServer';
    export const traceLevel = 'languageServer.traceLevel';
}
