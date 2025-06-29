// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";
import {
  ensureParentDirExists,
  expectFileExists,
  pathToExampleFile,
  pathToTempFile,
  readFileSync,
  writeTempFile,
} from "../utils/fs";

describe("bicep publish-extension", () => {
  it("should publish extension to the file system", () => {
    const indexJsonPath = pathToExampleFile("extensions.prod", "types", "http", "index.json");
    const targetPath = pathToTempFile("publish-extension", "extension.tgz");
    ensureParentDirExists(targetPath);

    invokingBicepCommand("publish-extension", "--target", targetPath, indexJsonPath).shouldSucceed().withEmptyStdout();

    expectFileExists(targetPath);

    const bicepContents = readFileSync(pathToExampleFile("extensions.prod", "main.bicep")).replace(
      "$TARGET_REFERENCE",
      "./extension.tgz",
    );
    const bicepPath = writeTempFile("publish-extension", "main.bicep", bicepContents);

    const configContents = readFileSync(pathToExampleFile("extensions.prod", "bicepconfig.json"));
    writeTempFile("publish-extension", "bicepconfig.json", configContents);

    // Building with --stdout should emit a valid result.
    invokingBicepCommand("build", "--stdout", bicepPath).shouldSucceed().withNonEmptyStdout();
  });
});
