// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { removeWhitespace } from "./removeWhitespace";

export function areEqualIgnoringWhitespace(a: string, b: string): boolean {
  return removeWhitespace(a) === removeWhitespace(b);
}
