// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Example } from "./example";

describe("msbuild", () => {
  it("multiple", () => {
    const example = new Example("multiple");
    example.clean();

    const result = example.build();

    expect(result.stderr).toBe("");

    example.expectTemplate("bin/Debug/templates/empty.json");
    example.expectTemplate("bin/Debug/templates/passthrough.json");
    example.expectTemplate("bin/Debug/templates/special/special.arm");
  });
});
