// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function isWhitespaceChar(ch: string): boolean {
  return /^\s$/.test(ch);
}
