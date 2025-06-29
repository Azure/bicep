// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import * as asserts from "./asserts";
import { Example } from "./example";

describe("msbuild", () => {
  it("should build and clean project with single .bicepparam file", () => {
    const example = new Example("bicepparam");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");
    const templateRelativePath = "bin/Debug/net472/main.parameters.json";
    example.expectTemplate(templateRelativePath);

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");
    example.expectNoFile(templateRelativePath);

    example.cleanProjectDir();

    example.publish(null);
    expect(buildResult.stderr).toBe("");

    // both build and publish outputs should be present
    example.expectTemplate(templateRelativePath);
    example.expectTemplate("bin/Debug/net472/publish/main.parameters.json");
  });

  it("fails and returns info for an incorrect file extension", () => {
    const example = new Example("bicepparam-bad-extension");
    example.cleanProjectDir();

    const result = example.build(false);

    expect(result.status).not.toBe(0);

    asserts.expectLinesInLog(result.stdout, [
      "0 Warning(s)",
      "1 Error(s)",
      " was not recognized as a Bicep Parameters file. Bicep parameters files must use the .bicepparam extension. ",
    ]);
  });

  it("build failures should produce expected diagnostics", () => {
    const example = new Example("bicepparam-errors");
    example.cleanProjectDir();

    const result = example.build(false);

    expect(result.status).toBe(1);

    asserts.expectLinesInLog(result.stdout, [
      "1 Warning(s)",
      "4 Error(s)",
      'main.bicepparam(1,7): error BCP258: The following parameters are declared in the Bicep file but are missing an assignment in the params file: "extraneous"',
      "main.bicepparam(1,7): error BCP104: The referenced module has errors.",
      'main.bicepparam(3,1): error BCP259: The parameter "missing" is assigned in the params file without being declared in the Bicep file.',
      'main.bicep(1,7): warning no-unused-params: Parameter "extraneous" is declared but never used. [https://aka.ms/bicep/linter-diagnostics#no-unused-params]',
      'main.bicep(3,27): error BCP033: Expected a value of type "object" but the provided value is of type "\'\'"',
    ]);
  });
});
