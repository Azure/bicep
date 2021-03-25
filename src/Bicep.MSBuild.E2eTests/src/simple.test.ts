// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Example } from "./example";

describe("msbuild", () => {
  it("simple", () => {
    const example = new Example("simple");
    example.clean();

    const result = example.build();

    expect(result.stderr).toBe("");

    example.expectTemplate("bin/Debug/net472/empty.json");
  });
});
