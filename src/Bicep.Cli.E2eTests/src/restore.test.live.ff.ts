// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep restore".
 *
 * @group live/ff
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
  fairfax,
  fairfaxEnvironmentOverrides,
} from "./utils/liveTestEnvironments";

const testArea = "restore";

async function emptyModuleCacheRoot() {
  await emptyDir(moduleCacheRoot);
}

describe("bicep restore", () => {
  beforeEach(emptyModuleCacheRoot);

  it("should restore template specs", () => {
    const bicep = `
module storageAccountModuleV1 'ts:${fairfax.templateSpecSubscriptionId}/bicep-ci/storageAccountSpec-${fairfax.resourceSuffix}:v1' = {
  name: 'storageAccountModuleV1'
  params: {
    sku: 'Standard_LRS'
  }
}

module storageAccountModuleV2 'ts/test-specs:STORAGEACCOUNTSPEC-${fairfax.resourceSuffix}:V2' = {
  name: 'storageAccountModuleV2'
  params: {
    sku: 'Standard_GRS'
    location: 'westus'
  }
}

module webAppModuleV1 'ts/test-specs:webAppSpec-${fairfax.resourceSuffix}:1.0.0' = {
  name: 'webAppModuleV1'
}
`;

    const bicepPath = writeTempFile("restore-ts", "main.bicep", bicep);
    const exampleConfig = readFileSync(
      pathToExampleFile("modules" + fairfax.suffix, "bicepconfig.json")
    );

    writeTempFile("restore-ts", "bicepconfig.json", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(fairfaxEnvironmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(
      pathToCachedTsModuleFile(
        `${fairfax.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${fairfax.resourceSuffix}/v1`,
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        `${fairfax.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${fairfax.resourceSuffix}/v2`,
        "main.json"
      )
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        `${fairfax.templateSpecSubscriptionId}/bicep-ci/webappspec-${fairfax.resourceSuffix}/1.0.0`,
        "main.json"
      )
    );
  });

  it("should restore OCI artifacts", () => {
    const builder = new BicepRegistryReferenceBuilder(
      fairfax.registryUri,
      testArea
    );

    const storageRef = builder.getBicepReference("storage", "v1");
    publishModule(
      fairfaxEnvironmentOverrides,
      storageRef,
      "modules" + fairfax.suffix,
      "storage.bicep"
    );

    const passthroughRef = builder.getBicepReference("passthrough", "v1");
    publishModule(
      fairfaxEnvironmentOverrides,
      passthroughRef,
      "modules" + fairfax.suffix,
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

module mcrModule 'br/public:samples/hello-world:1.0.1' = {
  name: 'mcrModule'
  params: {
    name: 'BicepE2E'
  }
}

output blobEndpoint string = storage.outputs.blobEndpoint
`;

    const bicepPath = writeTempFile("restore-br", "main.bicep", bicep);

    const exampleConfig = readFileSync(
      pathToExampleFile("modules" + fairfax.suffix, "bicepconfig.json")
    );
    writeTempFile("restore-br", "bicepconfig.json", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(fairfaxEnvironmentOverrides)
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

    expectBrModuleStructure(
      "mcr.microsoft.com",
      "bicep$samples$hello-world",
      "1.0.1$"
    );
  });
});
