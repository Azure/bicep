// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";
import { expectFileExists, pathToTempFile, writeTempFile } from "../utils/fs";

describe("bicep snapshot", () => {
  const testArea = "snapshot";

  it("should generate a snapshot file from a bicepparam file", () => {
    writeTempFile(testArea, "main.bicep", "param p string = 'x'\noutput o string = p\n");
    const bicepparamPath = writeTempFile(testArea, "main.bicepparam", "using 'main.bicep'\n");

    invokingBicepCommand("snapshot", bicepparamPath).shouldSucceed().withEmptyStdout();

    expectFileExists(pathToTempFile(testArea, "main.snapshot.json"));
  });

  it("should show error message when no input file path was specified", () => {
    invokingBicepCommand("snapshot")
      .shouldFail()
      .withStderr(/Required argument missing for command: 'snapshot'/);
  });
});
