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
  readFileSync,
} from "./utils/fs";
import {
  environments,
  createEnvironmentOverrides,
} from "./utils/liveTestEnvironments";

const testArea = "restore";

async function emptyModuleCacheRoot() {
  await emptyDir(moduleCacheRoot);
}

describe("bicep restore", () => {
  beforeEach(emptyModuleCacheRoot);

  it.each(environments)("should restore template specs (%p)", (environment) => {
    const bicep = `
module storageAccountModuleV1 'ts:${environment.templateSpecSubscriptionId}/bicep-ci/storageAccountSpec-${environment.resourceSuffix}:v1' = {
  name: 'storageAccountModuleV1'
  params: {
    sku: 'Standard_LRS'
  }
}

module storageAccountModuleV2 'ts/test-specs:STORAGEACCOUNTSPEC-${environment.resourceSuffix}:V2' = {
  name: 'storageAccountModuleV2'
  params: {
    sku: 'Standard_GRS'
    location: 'westus'
  }
}

module webAppModuleV1 'ts/test-specs:webAppSpec-${environment.resourceSuffix}:1.0.0' = {
  name: 'webAppModuleV1'
}`;

    const bicepPath = writeTempFile("restore-ts", "main.bicep", bicep);
    const exampleConfig = readFileSync(
      pathToExampleFile("modules" + environment.suffix, "bicepconfig.json")
    );

    writeTempFile("restore-ts", "bicepconfig.json", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(createEnvironmentOverrides(environment))
      .shouldSucceed()
      .withEmptyStdout();

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

    const passthroughWithRegistryAliasRef = builder.getBicepReferenceWithAlias(
      "test-registry",
      "restore/passthrough",
      "v1"
    );

    const passthroughWithFullAliasRef = builder.getBicepReferenceWithAlias(
      "test-modules",
      "passthrough",
      "v1"
    );

    const bicep = `
module passthrough '${passthroughRef}' = {
  name: 'passthrough'
  params: {
    text: 'hello'
    number: 42
  }
}

module passthroughWithRegistryAlias '${passthroughWithRegistryAliasRef}' = {
  name: 'passthroughWithRegistryAlias'
  params: {
    text: 'hello'
    number: 42
  }
}

module passthroughWithFullAlias '${passthroughWithFullAliasRef}' = {
  name: 'passthroughWithFullAlias'
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

    const bicepPath = writeTempFile("restore-br", "main.bicep", bicep);

    const exampleConfig = readFileSync(
      pathToExampleFile("modules" + environment.suffix, "bicepconfig.json")
    );
    writeTempFile("restore-br", "bicepconfig.json", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(environmentOverrides)
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
