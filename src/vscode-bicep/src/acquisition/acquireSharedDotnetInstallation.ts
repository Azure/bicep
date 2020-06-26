/* --------------------------------------------------------------------------------------------
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 * ------------------------------------------------------------------------------------------ */

import { commands } from 'vscode';

interface IDotnetAcquireResult {
    dotnetPath: string;
}

// Returns undefined if acquisition fails.
export async function acquireSharedDotnetInstallation(version: string): Promise<string | undefined> {

    let message: string | undefined;
    let result: IDotnetAcquireResult | undefined;

    return new Promise(async (resolve, reject) => {
        try {
            result = await commands.executeCommand<IDotnetAcquireResult>('dotnet.acquire', { version });
            return result.dotnetPath;
        } catch (err) {
            console.log(err);
        }
    });
}