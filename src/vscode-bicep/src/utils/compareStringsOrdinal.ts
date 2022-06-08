// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function compareStringsOrdinal(a: string, b: string): number {
  if (a > b) {
    return 1;
  } else if (b > a) {
    return -1;
  } else {
    return 0;
  }
}
