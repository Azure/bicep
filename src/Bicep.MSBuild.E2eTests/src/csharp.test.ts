// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Example } from "./example";

describe("msbuild", () => {
  it("should build and clean a c# project with Bicep files", () => {
    const example = new Example("csharp", "csharp.csproj");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");

    example.expectFile("bin/Debug/net7.0/csharp.dll");
    const templateRelativePath = "bin/Debug/net7.0/empty.json";
    example.expectTemplate(templateRelativePath);

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");
    example.expectNoFile(templateRelativePath);

    example.cleanProjectDir();

    example.publish(null);
    expect(buildResult.stderr).toBe("");

    // both build and publish outputs should be present
    example.expectTemplate(templateRelativePath);
    example.expectTemplate("bin/Debug/net7.0/publish/empty.json");
  });
});
