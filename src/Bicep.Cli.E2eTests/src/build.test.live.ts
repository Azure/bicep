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
import {
  expectFileExists,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  pathToTempFile,
  readFileSync,
  writeTempFile,
} from "./utils/fs";
import {
  createEnvironmentOverrides,
  environments,
} from "./utils/liveTestEnvironments";

describe("bicep build", () => {
  const testArea = "build";

  it.each(environments)(
    "should fail to build with --no-restore switch if modules are not cached (%p)",
    (environment) => {
      const bicep = `
module test 'br:${environment.registryUri}/does-not-exist:v-never' = {
  name: 'test'
}
    `;

      const bicepPath = writeTempFile("build", "no-restore.bicep", bicep);
      invokingBicepCommand("build", bicepPath, "--no-restore")
        .shouldFail()
        .withStderr(
          /.+BCP190: The module with reference "br:biceptest.+\.azurecr\..+\/does-not-exist:v-never" has not been restored..*/
        );
    }
  );

  it.each(environments)(
    "should build file with external modules (%p)",
    (environment) => {
      const builder = new BicepRegistryReferenceBuilder(
        environment.registryUri,
        testArea
      );

      const environmentOverrides = createEnvironmentOverrides(environment);
      const storageRef = builder.getBicepReference("storage", "v1");
      publishModule(
        environmentOverrides,
        storageRef,
        "modules" + environment.suffix,
        "storage.bicep"
      );

      const passthroughRef = builder.getBicepReference("passthrough", "v1");
      publishModule(
        environmentOverrides,
        passthroughRef,
        "modules" + environment.suffix,
        "passthrough.bicep"
      );

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

module webAppModuleV1 'ts/test-specs:webAppSpec-${environment.resourceSuffix}:1.0.0' = {
  name: 'webAppModuleV1'
}

output blobEndpoint string = storage.outputs.blobEndpoint
    `;

      const bicepPath = writeTempFile("build", "build-external.bicep", bicep);

      const exampleConfig = readFileSync(
        pathToExampleFile("modules" + environment.suffix, "bicepconfig.json")
      );
      writeTempFile("build", "bicepconfig.json", exampleConfig);

      invokingBicepCommand("build", bicepPath)
        .withEnvironmentOverrides(environmentOverrides)
        .shouldSucceed()
        .withEmptyStdout();

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

      expectFileExists(
        pathToCachedTsModuleFile(
          `${environment.templateSpecSubscriptionId}/bicep-ci/webappspec-${environment.resourceSuffix}/1.0.0`,
          "main.json"
        )
      );
    }
  );
});
