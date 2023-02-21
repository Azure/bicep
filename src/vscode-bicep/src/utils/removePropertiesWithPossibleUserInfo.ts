// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

const deployParamsPattern = new RegExp(
  '(?<lhs>"token":\\s*")(?<token>[^"]+)"',
  "g"
);

export function removePropertiesWithPossibleUserInfoInDeployParams(
  value: string
): string {
  return value.replace(deployParamsPattern, '$<lhs><REDACTED: token>"');
}
