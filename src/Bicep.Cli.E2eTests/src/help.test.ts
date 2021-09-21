// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep help".
 *
 * @group CI
 */

import { invokingBicepCommand } from "./utils/command";

describe("bicep --help", () => {
  it("should output help information", () => {
    invokingBicepCommand("--help").shouldSucceed().withNonEmptyStdout();
  });
});
