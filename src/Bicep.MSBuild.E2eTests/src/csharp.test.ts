// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { Example } from "./example";

describe("msbuild", () => {
  it("should build and clean a c# project with Bicep files", () => {
    const example = new Example("csharp", "csharp.csproj");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");

    example.expectFile("bin/Debug/net10.0/csharp.dll");
    const templateRelativePath = "bin/Debug/net10.0/empty.json";
    example.expectTemplate(templateRelativePath);

    const templateRelativePath2 = "bin/Debug/net10.0/empty2.json";
    example.expectTemplate(templateRelativePath2);

    const skipFileRelativePath = "bin/Debug/net10.0/skip.json";
    example.expectNoFile(skipFileRelativePath);

    const skipFileRelativePath2 = "bin/Debug/net10.0/skip2.json";
    example.expectNoFile(skipFileRelativePath2);

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");
    example.expectNoFile(templateRelativePath);
    example.expectNoFile(templateRelativePath2);
    example.expectNoFile(skipFileRelativePath);
    example.expectNoFile(skipFileRelativePath2);

    example.cleanProjectDir();

    example.publish(null);
    expect(buildResult.stderr).toBe("");

    // both build and publish outputs should be present
    example.expectTemplate("bin/Release/net10.0/empty.json");
    example.expectTemplate("bin/Release/net10.0/empty2.json");
    example.expectTemplate("bin/Release/net10.0/publish/empty.json");
    example.expectTemplate("bin/Release/net10.0/publish/empty2.json");
    example.expectNoFile("bin/Release/net10.0/publish/skip.json");
    example.expectNoFile("bin/Release/net10.0/publish/skip2.json");
  });
});
