// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { normalizeIndentation } from "./normalizeIndentation";
import { normalizeLineEndings } from "./normalizeLineEndings";

export function normalizeMultilineString(s: string, spacesPerTab = 1): string {
  return normalizeLineEndings(normalizeIndentation(s, spacesPerTab));
}
