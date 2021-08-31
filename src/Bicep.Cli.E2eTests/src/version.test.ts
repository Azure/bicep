// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { runBicepCommand } from "./command";

describe("bicep --version", () => {
  it("should output version information", () => {
    const result = runBicepCommand(["--version"]);
    expect(result.status).toBe(0);
    expect(result.stdout.length).toBeGreaterThan(0);
  });
});
