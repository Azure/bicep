// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep restore".
 *
 * @group Live
 */

import { invokingBicepCommand } from "./utils/command";
import {
  moduleCacheRoot,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  emptyDir,
  expectFileExists,
} from "./utils/fs";

async function emptyModuleCacheRoot() {
  await emptyDir(moduleCacheRoot);
}

describe("bicep restore", () => {
  beforeEach(emptyModuleCacheRoot);

  afterAll(emptyModuleCacheRoot);

  it("should restore external modules referenced in a bicep file", () => {
    const exampleFilePath = pathToExampleFile("external-modules", "main.bicep");
    invokingBicepCommand("restore", exampleFilePath)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(
      pathToCachedTsModuleFile(
        "61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/storageAccountSpec-df/v1",
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        "61E0A28A-63ED-4AFC-9827-2ED09B7B30F3/BICEP-CI/STORAGEACCOUNTSPEC-DF/V2",
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        "61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/webAppSpec-df/1.0.0",
        "main.json"
      )
    );
  });
});
