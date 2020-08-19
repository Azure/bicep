/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */
import { commands } from 'vscode';

export async function acquireSharedDotnetInstallation(version: string): Promise<string | undefined> {
    const result = await commands.executeCommand<{ dotnetPath: string }>('dotnet.acquire', { version });

    return result?.dotnetPath;            
}