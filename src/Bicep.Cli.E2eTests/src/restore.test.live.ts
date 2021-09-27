// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep restore".
 *
 * @group Live
 */

import {
  BicepRegistryReferenceBuilder,
  expectBrModuleStructure,
  publishModule,
} from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import {
  moduleCacheRoot,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  emptyDir,
  expectFileExists,
  writeTempFile,
} from "./utils/fs";

async function emptyModuleCacheRoot() {
  await emptyDir(moduleCacheRoot);
}

describe("bicep restore", () => {
  beforeEach(emptyModuleCacheRoot);

  //afterAll(emptyModuleCacheRoot);

  it("should restore template specs", () => {
    const exampleFilePath = pathToExampleFile("external-modules", "main.bicep");
    invokingBicepCommand("restore", exampleFilePath)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(
      pathToCachedTsModuleFile(
        "61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/storageaccountspec-df/v1",
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        "61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/storageaccountspec-df/v2",
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        "61e0a28a-63ed-4afc-9827-2ed09b7b30f3/bicep-ci/webappspec-df/1.0.0",
        "main.json"
      )
    );
  });

  it("should restore OCI artifacts", () => {
    const registry = "biceptestdf.azurecr.io";
    const builder = new BicepRegistryReferenceBuilder(registry, "restore");

    const storageRef = builder.getBicepReference("storage", "v1");
    publishModule(storageRef, "local-modules", "storage.bicep");

    const passthroughRef = builder.getBicepReference("passthrough", "v1");
    publishModule(passthroughRef, "local-modules", "passthrough.bicep");

    const bicep = `
module passthrough '${passthroughRef}' = {
  name: 'passthrough'
  params: {
    text: 'hello'
    number: 42
  }
}

module storage '${storageRef}' = {
  name: 'storage'
  params: {
    name: passthrough.outputs.result
  }
}

output blobEndpoint string = storage.outputs.blobEndpoint
    `;

    const bicepPath = writeTempFile("restore", "main.bicep", bicep);
    invokingBicepCommand("restore", bicepPath)
      .shouldSucceed()
      .withEmptyStdout();

    expectBrModuleStructure(
      builder.registry,
      "restore$passthrough",
      `v1_${builder.tagSuffix}$4002000`
    );

    expectBrModuleStructure(
      builder.registry,
      "restore$storage",
      `v1_${builder.tagSuffix}$4002000`
    );
  });
});
