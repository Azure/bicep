// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Example } from "./example";

describe("msbuild", () => {
  it("multiple", () => {
    const example = new Example("multiple");
    example.clean();

    const result = example.build();

    expect(result.stderr).toBe("");

    example.expectTemplate("bin/Debug/templates/net6.0/empty.json");
    example.expectTemplate("bin/Debug/templates/net6.0/passthrough.json");
    example.expectTemplate("bin/Debug/templates/net6.0/special/special.arm");
  });
});
