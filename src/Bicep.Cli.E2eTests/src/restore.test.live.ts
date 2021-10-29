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
import { invokingBicepCommandWithEnvOverrides } from "./utils/command";
import {
  moduleCacheRoot,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  emptyDir,
  expectFileExists,
  writeTempFile,
  readFileSync,
  logFiles,
} from "./utils/fs";
import {
  environments,
  createEnvironmentOverrides,
} from "./utils/liveTestEnvironments";

async function emptyModuleCacheRoot() {
  await emptyDir(moduleCacheRoot);
}

describe("bicep restore", () => {
  beforeEach(emptyModuleCacheRoot);

  const testArea = "restore";

  // TODO: Referenced file has direct module refs
  it.each(environments)("should restore template specs (%p)", (environment) => {
    const exampleFilePath = pathToExampleFile(
      "external-modules" + environment.suffix,
      "main.bicep"
    );
    invokingBicepCommandWithEnvOverrides(
      createEnvironmentOverrides(environment),
      "restore",
      exampleFilePath
    )
      .shouldSucceed()
      .withEmptyStdout();

    logFiles(pathToCachedTsModuleFile());

    expectFileExists(
      pathToCachedTsModuleFile(
        `${environment.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${environment.resourceSuffix}/v1`,
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        `${environment.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${environment.resourceSuffix}/v2`,
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        `${environment.templateSpecSubscriptionId}/bicep-ci/webappspec-${environment.resourceSuffix}/1.0.0`,
        "main.json"
      )
    );
  });

  it.each(environments)("should restore OCI artifacts (%p)", (environment) => {
    const builder = new BicepRegistryReferenceBuilder(
      environment.registryUri,
      testArea
    );

    const envOverrides = createEnvironmentOverrides(environment);
    const storageRef = builder.getBicepReference("storage", "v1");
    publishModule(
      envOverrides,
      storageRef,
      "local-modules" + environment.suffix,
      "storage.bicep"
    );

    const passthroughRef = builder.getBicepReference("passthrough", "v1");
    publishModule(
      envOverrides,
      passthroughRef,
      "local-modules" + environment.suffix,
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

output blobEndpoint string = storage.outputs.blobEndpoint
    `;

    const bicepPath = writeTempFile("restore", "main.bicep", bicep);

    const exampleConfig = readFileSync(
      pathToExampleFile(
        "local-modules" + environment.suffix,
        "bicepconfig.json"
      )
    );
    writeTempFile("restore", "bicepconfig.json", exampleConfig);

    invokingBicepCommandWithEnvOverrides(envOverrides, "restore", bicepPath)
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
