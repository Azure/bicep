// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import * as asserts from "./asserts";
import { Example } from "./example";

describe("msbuild", () => {
  it("should promote info diagnostic to warning", () => {
    const example = new Example("info-to-warning");
    example.cleanProjectDir();

    const result = example.build(false);
    asserts.expectLinesInLog(result.stdout, [
      "1 Warning(s)",
      "0 Error(s)",
      'main.bicep(3,5): warning no-unused-vars: Variable "unused" is declared but never used.',
    ]);
  });

  it("should promote info diagnostic to error", () => {
    const example = new Example("info");
    example.cleanProjectDir();

    const result = example.build(false);
    asserts.expectLinesInLog(result.stdout, ["0 Warning(s)", "0 Error(s)"]);
  });
});
