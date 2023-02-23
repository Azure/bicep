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

  it("should handle multiple replacements", () => {
    let value =
      'Params: {    "textDocument": {        "uri": "someUri"    }, "token": "eyJ0eXAi",    "expiresOnTimestamp": "1648143343698"}';
    value = value.repeat(10) + "\n" + value.repeat(10);
    const actual = removePropertiesWithPossibleUserInfoInDeployParams(value);
    let expected =
      'Params: {    "textDocument": {        "uri": "someUri"    }, "token": "<REDACTED: token>",    "expiresOnTimestamp": "1648143343698"}';
    expected = expected.repeat(10) + "\n" + expected.repeat(10);

    expect(actual).toMatch(expected);
  });

  it("should not blow up on large strings", () => {
    const withToken =
      'Params: {\n    "textDocument": {\n        "uri": "someUri"\n    }, "token": "eyJ0eXAi",\n    "expiresOnTimestamp": "1648143343698"\n}';

    const string10K = Array(10000)
      .fill(null)
      .map(() =>
        "    ======\"'{}()!@#$%^&*()_+][|;':,abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".charAt(
          Math.random() * 62
        )
      )
      .join("");
    const string10MB = (string10K + withToken).repeat(
      Math.ceil(10000000 / (string10K.length + withToken.length))
    );

    const start = Date.now();
    const actual =
      removePropertiesWithPossibleUserInfoInDeployParams(string10MB);
    const duration = Date.now() - start;

    expect(actual).not.toContain("eyJ0eXAi");
    console.log(duration);
    expect(duration).toBeLessThan(1000); // I normally see < 20ms, but giving some buffer
  });
});
