// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { BicepRegistryReferenceBuilder } from "../utils/br";
import { invokingBicepCommand } from "../utils/command";
import { pathToExampleFile } from "../utils/fs";
import { getEnvironment } from "../utils/liveTestEnvironments";

describe("bicep publish", () => {
  const testArea = "publish";
  const environment = getEnvironment();
  const builder = new BicepRegistryReferenceBuilder(environment.registryUri, testArea);

  it("should publish valid module", () => {
    const exampleFilePath = pathToExampleFile("101", "aks" + environment.suffix, "main.bicep");
    const target = builder.getBicepReference("aks", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed();
  });

  it("should publish valid module with alias", () => {
    const exampleFilePath = pathToExampleFile("101", "aks" + environment.suffix, "main.bicep");
    const target = builder.getBicepReferenceWithAlias("publish-alias", "aks", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target, "--force")
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed();
  });

  it("should fail to publish invalid module", () => {
    const exampleFilePath = pathToExampleFile("101", "aks" + environment.suffix, "flawed.bicep");
    const target = builder.getBicepReference("aks-flawed", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldFail()
      .withNonEmptyStderr();
  });
});
