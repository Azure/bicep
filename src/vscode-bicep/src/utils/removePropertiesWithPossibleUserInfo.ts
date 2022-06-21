// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
const deployParamsPattern = new RegExp('.*"token":\\s*"(?<token>.*)"');
export function removePropertiesWithPossibleUserInfoInDeployParams(
  value: string
): string {
  const matches = deployParamsPattern.exec(value);

  if (matches !== null) {
    const groups = matches.groups;

    if (groups !== null) {
      const token = groups["token"];

      return value.replace(token, "<REDACTED: token>");
    }
  }

  return value;
}
