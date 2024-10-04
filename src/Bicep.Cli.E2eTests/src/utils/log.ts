// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function logStdErr(data: string): void {
  for (const line of data.split("\n").filter((s: string) => !!s)) {
    (line.toLowerCase().startsWith("warning") ? console.log : console.error)(line);
  }
}
