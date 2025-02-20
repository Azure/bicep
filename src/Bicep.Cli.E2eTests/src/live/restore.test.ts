// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { BicepRegistryReferenceBuilder, expectBrModuleStructure, publishModule } from "../utils/br";
import { invokingBicepCommand } from "../utils/command";
import {
  emptyDir,
  // emptyDir,
  expectFileExists,
  pathToCachedBrModuleFile,
  // defaultModuleCacheRoot,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  pathToTempFile,
  readFileSync,
  writeTempFile,
} from "../utils/fs";
import { getEnvironment } from "../utils/liveTestEnvironments";

function writeTempBicepConfigFile(testArea: string, bicepConfigContents: string) {
  const bicepConfigJson = JSON.parse(bicepConfigContents);
  bicepConfigJson.cacheRootDirectory = pathToTempFile(testArea, ".bicep");
  bicepConfigContents = JSON.stringify(bicepConfigJson, null, 2);

  return writeTempFile(testArea, "bicepconfig.json", bicepConfigContents);
}

describe("bicep restore", () => {
  const testArea = "restore";
  const environment = getEnvironment();

  // beforeEach(emptyModuleCacheRoot);

  it("should restore template specs", () => {
    const cacheRootDir = pathToTempFile("restore-ts", ".bicep");

    emptyDir(cacheRootDir);

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
}
`;

    const bicepPath = writeTempFile("restore-ts", "main.bicep", bicep);
    const exampleConfig = readFileSync(pathToExampleFile("modules" + environment.suffix, "bicepconfig.json"));
    writeTempBicepConfigFile("restore-ts", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(
      pathToCachedTsModuleFile(
        cacheRootDir,
        `${environment.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${environment.resourceSuffix}/v1`,
        "main.json",
      ),
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        cacheRootDir,
        `${environment.templateSpecSubscriptionId}/bicep-ci/storageaccountspec-${environment.resourceSuffix}/v2`,
        "main.json",
      ),
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        cacheRootDir,
        `${environment.templateSpecSubscriptionId}/bicep-ci/webappspec-${environment.resourceSuffix}/1.0.0`,
        "main.json",
      ),
    );
  });

  it("should restore ACR modules", () => {
    const cacheRootDir = pathToTempFile("restore-br", ".bicep");
    emptyDir(cacheRootDir);

    const builder = new BicepRegistryReferenceBuilder(environment.registryUri, testArea);

    const storageRef = builder.getBicepReference("storage", "v1");
    publishModule(environment.environmentOverrides, storageRef, "modules" + environment.suffix, "storage.bicep");

    const passthroughRef = builder.getBicepReference("passthrough", "v1");
    publishModule(
      environment.environmentOverrides,
      passthroughRef,
      "modules" + environment.suffix,
      "passthrough.bicep",
    );

    const passthroughWithRegistryAliasRef = builder.getBicepReferenceWithAlias(
      "test-registry",
      "restore/passthrough",
      "v1",
    );

    const passthroughWithFullAliasRef = builder.getBicepReferenceWithAlias("test-modules", "passthrough", "v1");

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

    const exampleConfig = readFileSync(pathToExampleFile("modules" + environment.suffix, "bicepconfig.json"));
    writeTempBicepConfigFile("restore-br", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectBrModuleStructure(cacheRootDir, builder.registry, "restore$passthrough", `v1_${builder.tagSuffix}$4002000`);

    expectBrModuleStructure(cacheRootDir, builder.registry, "restore$storage", `v1_${builder.tagSuffix}$4002000`);

    expectBrModuleStructure(cacheRootDir, "mcr.microsoft.com", "bicep$samples$hello-world", "1.0.1$");
  });

  it("should restore Graph extension", () => {
    const cacheRootDir = pathToTempFile("restore-graph-extension", ".bicep");
    emptyDir(cacheRootDir);

    const bicep = "extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview'";
    const bicepPath = writeTempFile("restore-graph-extension", "main.bicep", bicep);

    const exampleConfig = readFileSync(pathToExampleFile("extensions" + environment.suffix, "bicepconfig.json"));
    writeTempBicepConfigFile("restore-graph-extension", exampleConfig);

    invokingBicepCommand("restore", bicepPath)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(
      pathToCachedBrModuleFile(
        cacheRootDir,
        "mcr.microsoft.com",
        "bicep$extensions$microsoftgraph$v1.0",
        "0.1.8-preview$",
        "types.tgz",
      ),
    );
  });
});
