// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function normalizeLineEndings(s: string): string {
  return s.replace(/\r\n/g, "\n");
}
