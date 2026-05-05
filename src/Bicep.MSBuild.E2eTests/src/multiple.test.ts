// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { Example } from "./example";

describe("msbuild", () => {
  it("should build a multi-targeting project with customized output paths successfully", () => {
    const example = new Example("multiple");
    example.cleanProjectDir();

    const result = example.build();

    expect(result.stderr).toBe("");

    example.expectTemplate("bin/Debug/templates/net10.0/empty.json");
    example.expectTemplate("bin/Debug/templates/net10.0/passthrough.json");
    example.expectTemplate("bin/Debug/templates/net10.0/special/special.arm");
  });
});
