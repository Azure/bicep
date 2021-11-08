// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep publish".
 *
 * @group Live
 */

import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { pathToExampleFile } from "./utils/fs";
import {
  environments,
  createEnvironmentOverrides,
} from "./utils/liveTestEnvironments";

describe("bicep publish", () => {
  const testArea = "publish";
  it.each(environments)("should publish valid module (%p)", (environment) => {
    const builder = new BicepRegistryReferenceBuilder(
      environment.registryUri,
      testArea
    );
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + environment.suffix,
      "main.bicep"
    );
    const target = builder.getBicepReference("aks", "v1");
    console.log(`target = ${target}`);
    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(createEnvironmentOverrides(environment))
      .shouldSucceed();
  });

  it.each(environments)(
    "should publish valid module with alias (%p)",
    (environment) => {
      const builder = new BicepRegistryReferenceBuilder(
        environment.registryUri,
        testArea
      );
      const exampleFilePath = pathToExampleFile(
        "101",
        "aks" + environment.suffix,
        "main.bicep"
      );
      const target = builder.getBicepReferenceWithAlias(
        "publish-alias",
        "aks",
        "v1"
      );
      console.log(`target = ${target}`);
      invokingBicepCommand("publish", exampleFilePath, "--target", target)
        .withEnvironmentOverrides(createEnvironmentOverrides(environment))
        .shouldSucceed();
    }
  );

  it.each(environments)(
    "should fail to publish invalid module (%p)",
    (environment) => {
      const builder = new BicepRegistryReferenceBuilder(
        environment.registryUri,
        testArea
      );
      const exampleFilePath = pathToExampleFile(
        "101",
        "aks" + environment.suffix,
        "flawed.bicep"
      );
      const target = builder.getBicepReference("aks-flawed", "v1");
      console.log(`target = ${target}`);
      invokingBicepCommand("publish", exampleFilePath, "--target", target)
        .withEnvironmentOverrides(createEnvironmentOverrides(environment))
        .shouldFail()
        .withNonEmptyStderr();
    }
  );
});
