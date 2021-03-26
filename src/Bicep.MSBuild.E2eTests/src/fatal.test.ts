// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as asserts from "./asserts";
import { Example } from "./example";

describe("msbuild", () => {
  it("fatal bicep errors should produce an msbuild diagnostic", () => {
    const example = new Example("fatal");
    example.clean();

    const result = example.build(false);

    expect(result.status).not.toBe(0);

    asserts.expectLinesInLog(result.stdout, [
      "0 Warning(s)",
      "1 Error(s)",
      ": error : An error occurred reading file. Could not find file ",
    ]);
  });
});
