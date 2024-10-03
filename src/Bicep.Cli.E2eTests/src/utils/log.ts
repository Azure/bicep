// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable prettier/prettier */

export function logStdErr(data: string): void {
    for (const line of data.split("\n").filter((s: string) => !!s)) {
        (line.toLowerCase().startsWith("warning") ? console.log : console.error)(line)
    }
}
