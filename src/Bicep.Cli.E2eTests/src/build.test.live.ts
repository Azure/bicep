// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep build".
 *
 * @group Live
 */

import {
  BicepRegistryReferenceBuilder,
  expectBrModuleStructure,
  publishModule,
} from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { expectFileExists, pathToTempFile, writeTempFile } from "./utils/fs";

describe("bicep build", () => {
  const registry = "biceptestdf.azurecr.io";

  it("should fail to build with --no-restore switch if modules are not cached", () => {
    const bicep = `
module test 'br:${registry}/does-not-exist:v-never' = {
  name: 'test'
}
    `;

    const bicepPath = writeTempFile("build", "no-restore.bicep", bicep);
    invokingBicepCommand("build", bicepPath, "--no-restore")
      .shouldFail()
      .withStderr(
        /.+BCP190: The module with reference "br:biceptestdf.azurecr.io\/does-not-exist:v-never" has not been restored..*/
      );
  });

  it("should build file with external modules", () => {
    const registry = "biceptestdf.azurecr.io";
    const builder = new BicepRegistryReferenceBuilder(registry, "build");

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

    const bicepPath = writeTempFile("build", "build-external.bicep", bicep);
    invokingBicepCommand("build", bicepPath).shouldSucceed().withEmptyStdout();

    expectFileExists(pathToTempFile("build", "build-external.json"));

    expectBrModuleStructure(
      builder.registry,
      "build$passthrough",
      `v1_${builder.tagSuffix}$4002000`
    );

    expectBrModuleStructure(
      builder.registry,
      "build$storage",
      `v1_${builder.tagSuffix}$4002000`
    );
  });
});
