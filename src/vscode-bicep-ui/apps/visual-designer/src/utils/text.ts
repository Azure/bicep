// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function camelCaseToWords(value?: string) {
  if (!value) {
    return "";
  }

  const result = value.replace(/([A-Z])/g, ' $1');
  return result.charAt(0).toUpperCase() + result.slice(1);
}
