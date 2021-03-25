// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as asserts from "./asserts";
import { Example } from "./example";

describe("msbuild", () => {
  it("should produce a friendly error message if CLI is missing from the path", () => {
    const example = new Example("missingCli");
    example.clean();

    const result = example.build(false);

    expect(result.status).not.toBe(0);

    asserts.expectLinesInLog(result.stdout, [
      "0 Warning(s)",
      "1 Error(s)",
      ": error : The path to the Bicep compiler is not set. Reference the appropriate Azure.Bicep.CommandLine.* package for your runtime or set the BicepPath property.",
    ]);
  });
});
