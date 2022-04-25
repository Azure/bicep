// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function removeIndentation(s: string): string {
  return s.replace(/^ */gm, "");
}
