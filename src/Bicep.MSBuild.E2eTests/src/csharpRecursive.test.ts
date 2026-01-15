// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
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
  it("should build a c# project with recursive copy enabled", () => {
    const example = new Example("csharp-recursive", "csharp-recursive.csproj");
    example.cleanProjectDir();

    const result = example.build();

    expect(result.stderr).toBe("");

    const framework = "net10.0";
    getOutputFiles("Debug", framework, false).forEach((file) => example.expectTemplate(file));

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");

    getOutputFiles("Debug", framework, false).forEach((file) => example.expectNoFile(file));

    const publishResult = example.publish(framework);
    expect(publishResult.stderr).toBe("");

    // after publish we should expect build output
    // in the Release directory for the chosen framework
    getOutputFiles("Release", framework, false).forEach((file) => example.expectTemplate(file));

    // publish dir should be populated with the same content
    getOutputFiles("Release", framework, true).forEach((file) => example.expectTemplate(file));
  });
});
