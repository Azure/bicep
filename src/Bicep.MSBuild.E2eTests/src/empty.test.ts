// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, expect, it } from "vitest";
import { Example } from "./example";

describe("msbuild", () => {
  it("should build project without bicep file successfully", () => {
    const example = new Example("empty");
    example.cleanProjectDir();

    const result = example.build();

    expect(result.stderr).toBe("");
  });
});
