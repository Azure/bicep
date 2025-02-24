// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { beforeEach, describe, it } from "vitest";
import { BicepRegistryReferenceBuilder, expectBrModuleStructure, publishModule } from "../utils/br";
import { invokingBicepCommand } from "../utils/command";
import {
  defaultModuleCacheRoot,
  emptyDir,
  expectFileExists,
  pathToCachedTsModuleFile,
  pathToExampleFile,
  pathToTempFile,
  readFileSync,
  writeTempFile,
} from "../utils/fs";
import { getEnvironment } from "../utils/liveTestEnvironments";

async function emptyModuleCacheRoot() {
  await emptyDir(defaultModuleCacheRoot);
}

describe("bicep build", () => {
  const testArea = "build";
  const environment = getEnvironment();

  beforeEach(emptyModuleCacheRoot);

  it("should fail to build with --no-restore switch if modules are not cached", () => {
    const bicep = `
module test 'br:${environment.registryUri}/does-not-exist:v-never' = {
  name: 'test'
}
    `;

    const bicepPath = writeTempFile("build", "no-restore.bicep", bicep);
    invokingBicepCommand("build", bicepPath, "--no-restore")
      .shouldFail()
      .withStderr(
        /.+BCP190: The artifact with reference "br:biceptest.+\.azurecr\..+\/does-not-exist:v-never" has not been restored..*/,
      );
  });

  it("should build file with external modules", () => {
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

    const mainContent = `
module passthrough '${passthroughRef}' = {
  name: 'passthrough'
  params: {
    text: 'hello'
    number: 42
  }
}

module localModule 'build-external-local-module.bicep' = {
  name: 'localModule'
  params: {
    passthroughResult: passthrough.outputs.result
  }
}

module webAppModuleV1 'ts/test-specs:webAppSpec-${environment.resourceSuffix}:1.0.0' = {
  name: 'webAppModuleV1'
}

output blobEndpoint string = localModule.outputs.blobEndpoint`;

    const localModuleContent = `
param passthroughResult string

module storage '${storageRef}' = {
  name: 'storage'
  params: {
    name: passthroughResult
  }
}

module nestedLocalModule 'build-external-nested-local-module.bicep' = {
  name: 'nestedLocalModule'
}

output blobEndpoint string = storage.outputs.blobEndpoint`;

    const nestedLocalModuleContent = `
module webAppModuleV1 'ts/test-specs:webAppSpec-${environment.resourceSuffix}:1.0.0' = {
  name: 'webAppModuleV1'
}`;

    const bicepPath = writeTempFile("build", "build-external.bicep", mainContent);

    writeTempFile("build", "build-external-local-module.bicep", localModuleContent);

    writeTempFile("build", "build-external-nested-local-module.bicep", nestedLocalModuleContent);

    const exampleConfig = readFileSync(pathToExampleFile("modules" + environment.suffix, "bicepconfig.json"));
    writeTempFile("build", "bicepconfig.json", exampleConfig);

    invokingBicepCommand("build", bicepPath)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(pathToTempFile("build", "build-external.json"));

    expectBrModuleStructure(
      defaultModuleCacheRoot,
      builder.registry,
      "build$passthrough",
      `v1_${builder.tagSuffix}$4002000`,
    );

    expectBrModuleStructure(
      defaultModuleCacheRoot,
      builder.registry,
      "build$storage",
      `v1_${builder.tagSuffix}$4002000`,
    );

    expectFileExists(
      pathToCachedTsModuleFile(
        defaultModuleCacheRoot,
        `${environment.templateSpecSubscriptionId}/bicep-ci/webappspec-${environment.resourceSuffix}/1.0.0`,
        "main.json",
      ),
    );
  });

  it("should build file deeply nested external modules", () => {
    const builder = new BicepRegistryReferenceBuilder(environment.registryUri, testArea);

    const passthroughRef = builder.getBicepReference("passthrough", "v1");
    publishModule(
      environment.environmentOverrides,
      passthroughRef,
      "modules" + environment.suffix,
      "passthrough.bicep",
    );

    const nested0 = `
module nested1 'nested1.bicep' = {
  name: 'nested1'
}
`;

    const nested1 = `
module nested2 'nested2.bicep' = {
  name: 'nested2'
}
`;

    const nested2 = `
module passthrough '${passthroughRef}' = {
  name: 'passthrough'
  params: {
    text: 'hello'
    number: 42
  }
}
`;

    const bicepPath = writeTempFile("build", "nested0.bicep", nested0);
    writeTempFile("build", "nested1.bicep", nested1);
    writeTempFile("build", "nested2.bicep", nested2);

    invokingBicepCommand("build", bicepPath)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed()
      .withEmptyStdout();

    expectBrModuleStructure(
      defaultModuleCacheRoot,
      builder.registry,
      "build$passthrough",
      `v1_${builder.tagSuffix}$4002000`,
    );
  });
});
