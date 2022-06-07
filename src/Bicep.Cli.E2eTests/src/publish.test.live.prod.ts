// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { pathToExampleFile } from "./utils/fs";
import { prod } from "./utils/liveTestEnvironments";

/**
 * Live tests for "bicep publish".
 *
 * @group live/prod
 */

describe("bicep publish", () => {
  const testArea = "publish";
  const builder = new BicepRegistryReferenceBuilder(prod.registryUri, testArea);

  it("should publish valid module", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + prod.suffix,
      "main.bicep"
    );

    const target = builder.getBicepReference("aks", "v1");

    invokingBicepCommand(
      "publish",
      exampleFilePath,
      "--target",
      target
    ).shouldSucceed();
  });

  it("should publish valid module with alias (%p)", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + prod.suffix,
      "main.bicep"
    );

    const target = builder.getBicepReferenceWithAlias(
      "publish-alias",
      "aks",
      "v1"
    );

    invokingBicepCommand(
      "publish",
      exampleFilePath,
      "--target",
      target
    ).shouldSucceed();
  });

  it("should fail to publish invalid module", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + prod.suffix,
      "flawed.bicep"
    );

    const target = builder.getBicepReference("aks-flawed", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .shouldFail()
      .withNonEmptyStderr();
  });
});
