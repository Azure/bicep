// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep restore".
 *
 * @group CI
 */

import { invokingBicepCommand } from "./utils/command";

describe("bicep restore", () => {
  it("should show error message when no input file path was specified", () => {
    invokingBicepCommand("restore")
      .shouldFail()
      .withStderr(/The input file path was not specified/);
  });
});
