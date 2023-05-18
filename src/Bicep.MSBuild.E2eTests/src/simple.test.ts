// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Example } from "./example";

describe("msbuild", () => {
  it("should build and clean project with single Bicep file", () => {
    const example = new Example("simple");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");
    const templateRelativePath = "bin/Debug/net472/empty.json";
    example.expectTemplate(templateRelativePath);

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");
    example.expectNoFile(templateRelativePath);

    example.cleanProjectDir();

    example.publish(null);
    expect(buildResult.stderr).toBe("");

    // both build and publish outputs should be present
    example.expectTemplate(templateRelativePath);
    example.expectTemplate("bin/Debug/net472/publish/empty.json");
  });
});
