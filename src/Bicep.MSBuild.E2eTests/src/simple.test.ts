// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { Example } from "./example";

describe("msbuild", () => {
  it("should build and clean project with single Bicep file", () => {
    const example = new Example("simple");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");
    const templateRelativePath = "bin/Debug/net472/empty.json";
    example.expectTemplate(templateRelativePath);
    const skippedFileRelativePath = "bin/Debug/net472/skip.json";
    example.expectNoFile(skippedFileRelativePath);

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");
    example.expectNoFile(templateRelativePath);

    example.cleanProjectDir();

    const publishResult = example.publish(null);
    expect(publishResult.stderr).toBe("");

    // both build and publish outputs should be present
    example.expectTemplate(templateRelativePath);
    example.expectTemplate("bin/Debug/net472/publish/empty.json");

    // skipped file must not be present in either build or publish output directory
    example.expectNoFile(skippedFileRelativePath);
    example.expectNoFile("bin/Debug/net472/publish/skip.json");
  });
});
