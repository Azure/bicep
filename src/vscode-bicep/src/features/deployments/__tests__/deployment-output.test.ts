// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { removePropertiesWithPossibleUserInfoInDeployParams } from "../deployment-output";

describe("removePropertiesWithPossibleUserInfoInDeployParams", () => {
  test("returns input without a token unchanged", () => {
    const value =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    },\n    "position": {\n        "line": 12,\n        "character": 16\n    }\n}\n\n';
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);

    expect(actual).toBe(value);
  });

  test("returns empty input unchanged", () => {
    const value = "";
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);

    expect(actual).toBe(value);
  });

  test("redacts a token", () => {
    const value =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "eyJ0eXAi",\n    "expiresOnTimestamp": "1648143343698"\n}';
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);
    const expected =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "<REDACTED: token>",\n    "expiresOnTimestamp": "1648143343698"\n}';

    expect(actual).toBe(expected);
  });

  test("redacts every token", () => {
    let value =
      'Params: {    "textDocument": {        "uri": "someUri"    }, "token": "eyJ0eXAi",    "expiresOnTimestamp": "1648143343698"}';
    value = value.repeat(10) + "\n" + value.repeat(10);
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);
    let expected =
      'Params: {    "textDocument": {        "uri": "someUri"    }, "token": "<REDACTED: token>",    "expiresOnTimestamp": "1648143343698"}';
    expected = expected.repeat(10) + "\n" + expected.repeat(10);

    expect(actual).toBe(expected);
  });

  test("redacts tokens in large input", () => {
    const withToken =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "eyJ0eXAi",\n    "expiresOnTimestamp": "1648143343698"\n}';

    const padding = "abcdefghijklmnopqrstuvwxyz0123456789".repeat(300);
    const largeInput = (padding + withToken).repeat(1000);

    const actual = removePropertiesWithPossibleUserInfoInDeployParams(largeInput);

    expect(actual).not.toContain("eyJ0eXAi");
    expect(actual.match(/<REDACTED: token>/g)).toHaveLength(1000);
  });
});
