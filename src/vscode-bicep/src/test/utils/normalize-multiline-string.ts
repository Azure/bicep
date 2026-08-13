// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { normalizeIndentation } from "./normalize-indentation";
import { normalizeLineEndings } from "./normalize-line-endings";

export function normalizeMultilineString(s: string, spacesPerTab = 2): string {
  return normalizeLineEndings(normalizeIndentation(s, spacesPerTab));
}
