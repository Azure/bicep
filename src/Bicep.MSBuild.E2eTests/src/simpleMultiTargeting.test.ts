// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { Example } from "./example";

const runningInAzurePipelines = !!process.env.TF_BUILD;

describe.skipIf(runningInAzurePipelines)("msbuild", () => {
  it("should build a multi-targeting project with default output paths successfully", () => {
    const example = new Example("simpleMultiTarget");
    example.cleanProjectDir();

    const buildResult = example.build();
    expect(buildResult.stderr).toBe("");

    example.expectTemplate("bin/Debug/net10.0/empty.json");
    example.expectTemplate("bin/Debug/net10.0/passthrough.json");
    example.expectTemplate("bin/Debug/net10.0/theAnswer.json");

    example.expectTemplate("bin/Debug/net472/empty.json");
    example.expectTemplate("bin/Debug/net472/passthrough.json");
    example.expectTemplate("bin/Debug/net472/theAnswer.json");

    const cleanResult = example.clean();
    expect(cleanResult.stderr).toBe("");

    example.expectNoFile("bin/Debug/net10.0/empty.json");
    example.expectNoFile("bin/Debug/net10.0/passthrough.json");
    example.expectNoFile("bin/Debug/net10.0/theAnswer.json");

    example.expectNoFile("bin/Debug/net472/empty.json");
    example.expectNoFile("bin/Debug/net472/passthrough.json");
    example.expectNoFile("bin/Debug/net472/theAnswer.json");

    const publishResult = example.publish("net472");
    expect(publishResult.stderr).toBe("");

    example.expectNoFile("bin/Debug/net10.0/publish/empty.json");
    example.expectNoFile("bin/Debug/net10.0/publish/passthrough.json");
    example.expectNoFile("bin/Debug/net10.0/publish/theAnswer.json");

    example.expectTemplate("bin/Debug/net472/publish/empty.json");
    example.expectTemplate("bin/Debug/net472/publish/passthrough.json");
    example.expectTemplate("bin/Debug/net472/publish/theAnswer.json");
  });
});
