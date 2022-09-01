// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { removePropertiesWithPossibleUserInfoInDeployParams } from "../../utils/removePropertiesWithPossibleUserInfo";

describe("removePropertiesWithPossibleUserInfoInDeployParams()", () => {
  it("should return input as is if it doesn't contain token and timestamp", () => {
    const value =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    },\n    "position": {\n        "line": 12,\n        "character": 16\n    }\n}\n\n';
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);

    expect(actual).toMatch(value);
  });

  it("should return input as is if it contains empty string", () => {
    const value = "";
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);

    expect(actual).toMatch(value);
  });

  it("should replace token and expiresOnTimeStamp with REDACTED string", () => {
    const value =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "eyJ0eXAi",\n    "expiresOnTimestamp": "1648143343698"\n}';
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);
    const expected =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "<REDACTED: token>",\n    "expiresOnTimestamp": "1648143343698"\n}';

    expect(actual).toMatch(expected);
  });
});
