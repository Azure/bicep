// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep build".
 *
 * @group CI
 */

import { invokingBicepCommand } from "./utils/command";
import {
  expectFileExists,
  expectFileNotExists,
  pathToExampleFile,
  readFileSync,
} from "./utils/fs";

describe("bicep build", () => {
  it("should build a bicep file", () => {
    const bicepFilePath = pathToExampleFile("101", "aks", "main.bicep");
    invokingBicepCommand("build", bicepFilePath)
      .shouldSucceed()
      .withEmptyStdout();

    const jsonFilePath = pathToExampleFile("101", "aks", "main.json");
    expectFileExists(jsonFilePath);

    const jsonContents = readFileSync(jsonFilePath);
    expect(jsonContents.length).toBeGreaterThan(0);

    // Building with --stdout should emit consistent result.
    invokingBicepCommand("build", "--stdout", bicepFilePath)
      .shouldSucceed()
      .withStdout(jsonContents);
  });

  it("should log to stderr if a bicep file has errors", () => {
    const bicepFilePath = pathToExampleFile("101", "aks", "flawed.bicep");
    invokingBicepCommand("build", bicepFilePath)
      .shouldFail()
      .withNonEmptyStderr();

    const jsonFilePath = pathToExampleFile("101", "aks", "flawed.json");
    expectFileNotExists(jsonFilePath);
  });
});
