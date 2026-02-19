// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

export function camelCaseToWords(value?: string) {
  if (!value) {
    return "";
  }

  // Insert a space before:
  //  - a sequence of uppercase letters followed by an uppercase + lowercase (e.g., "XMLParser" → "XML Parser")
  //  - a single uppercase letter preceded by a lowercase letter or digit (e.g., "publicIp" → "public Ip")
  const result = value.replace(/([A-Z]+)([A-Z][a-z])/g, "$1 $2").replace(/([a-z0-9])([A-Z])/g, "$1 $2");
  return result.charAt(0).toUpperCase() + result.slice(1);
}
