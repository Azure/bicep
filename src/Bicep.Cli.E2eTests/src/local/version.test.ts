// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep version".
 *
 * @group CI
 */

import { describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";

describe("bicep --version", () => {
  it("should output version information", () => {
    invokingBicepCommand("--version").shouldSucceed().withNonEmptyStdout();
  });
});
