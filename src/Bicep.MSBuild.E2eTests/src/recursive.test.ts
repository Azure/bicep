// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import * as asserts from "./asserts";
import { Example } from "./example";

function getOutputFiles(configuration: "Debug" | "Release", framework: string, publish: boolean): string[] {
  const publishPart = publish ? "publish/" : "";
  return [
    // templates
    `bin/${configuration}/${framework}/${publishPart}root.json`,
    `bin/${configuration}/${framework}/${publishPart}one/one.json`,
    `bin/${configuration}/${framework}/${publishPart}one/two/two.json`,

    // compiles params
    `bin/${configuration}/${framework}/${publishPart}root1.parameters.json`,
    `bin/${configuration}/${framework}/${publishPart}root2.parameters.json`,
    `bin/${configuration}/${framework}/${publishPart}one/one1.parameters.json`,
    `bin/${configuration}/${framework}/${publishPart}one/one2.parameters.json`,
    `bin/${configuration}/${framework}/${publishPart}one/two/two1.parameters.json`,
    `bin/${configuration}/${framework}/${publishPart}one/two/two2.parameters.json`,
  ];
}

describe("msbuild", () => {
  it("should build a multi-targeting project with recursive copy enabled", () => {
    const example = new Example("recursive");
    example.cleanProjectDir();

    const result = example.build();

    expect(result.stderr).toBe("");

    // TODO: ADO build for some reason refuses to build net472 when it's included in the array below
    // but the same works in GitHub actions. we need to add it back once we figure out why
    const targetFrameworks = ["net10.0"];

    targetFrameworks.forEach((framework: string): void => {
      getOutputFiles("Debug", framework, false).forEach((file) => example.expectTemplate(file));
    });

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");

    targetFrameworks.forEach((framework: string): void => {
      getOutputFiles("Debug", framework, false).forEach((file) => example.expectNoFile(file));
    });

    const publishFramework = targetFrameworks[0];
    const publishResult = example.publish(publishFramework);
    expect(publishResult.stderr).toBe("");

    // after publish we should expect build output
    // in the Release directory for the chosen framework
    getOutputFiles("Release", publishFramework, false).forEach((file) => example.expectTemplate(file));

    // publish dir should be populated with the same content
    getOutputFiles("Release", publishFramework, true).forEach((file) => example.expectTemplate(file));
  });

  it("should fail if BicepOutputStyle is not set to a valid value", () => {
    const example = new Example("recursive-badmode");
    example.cleanProjectDir();

    const result = example.build(false);
    expect(result.status).not.toBe(0);

    asserts.expectLinesInLog(result.stdout, [
      ": error : The BicepOutputStyle property must be set to either 'Flat' or 'Recursive'.",
      "0 Warning(s)",
      "1 Error(s)",
    ]);
  });
});
