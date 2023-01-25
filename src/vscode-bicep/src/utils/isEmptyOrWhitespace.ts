// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function isEmptyOrWhitespace(s: string): boolean {
  return /^\s*$/.test(s);
}
