// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export function removePropertiesWithPossibleUserInfoInDeployParams(value: string) {
  const deployParamsPattern = new RegExp(
    '.*"token":\\s*"(?<token>.*)",\\s.*"expiresOnTimestamp":\\s*"(?<expiresOnTimestamp>.*)".*'
  );
  const matches = deployParamsPattern.exec(value);

  if (matches != null) {
    const groups = matches.groups;

    if (groups != null) {
      const token = groups["token"];
      const expiresOnTimestamp = groups["expiresOnTimestamp"];

      let updatedValue = value.replace(token, "<REDACTED: token>");
      return updatedValue.replace(
        expiresOnTimestamp,
        "<REDACTED: expiresOnTimestamp>"
      );
    }
  }

  return value;
}
