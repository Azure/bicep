// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";

describe("bicep --help", () => {
  it("should output help information", () => {
    invokingBicepCommand("--help").shouldSucceed().withNonEmptyStdout();
  });
});
