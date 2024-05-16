// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep publish-provider".
 *
 * @group CI
 */

import { invokingBicepCommand } from "./utils/command";
import {
  ensureParentDirExists,
  expectFileExists,
  pathToExampleFile,
  pathToTempFile,
  readFileSync,
  writeTempFile,
} from "./utils/fs";

describe("bicep publish-provider", () => {
  it("should publish provider to the file system", () => {
    const indexJsonPath = pathToExampleFile(
      "providers.prod",
      "types",
      "http",
      "index.json",
    );
    const targetPath = pathToTempFile("publish-provider", "provider.tgz");
    ensureParentDirExists(targetPath);

    invokingBicepCommand(
      "publish-provider",
      "--target",
      targetPath,
      indexJsonPath,
    )
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(targetPath);

    const bicepContents = readFileSync(
      pathToExampleFile("providers.prod", "main.bicep"),
    ).replace("$TARGET_REFERENCE", "./provider.tgz");
    const bicepPath = writeTempFile(
      "publish-provider",
      "main.bicep",
      bicepContents,
    );

    const configContents = readFileSync(
      pathToExampleFile("providers.prod", "bicepconfig.json"),
    );
    writeTempFile("publish-provider", "bicepconfig.json", configContents);

    // Building with --stdout should emit a valid result.
    invokingBicepCommand("build", "--stdout", bicepPath)
      .shouldSucceed()
      .withNonEmptyStdout();
  });
});
