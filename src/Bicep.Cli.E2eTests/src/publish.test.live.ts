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

describe("bicep publish", () => {
  const builder = new BicepRegistryReferenceBuilder(
    "biceptestdf.azurecr.io",
    "publish"
  );

  it("should publish valid module", () => {
    const exampleFilePath = pathToExampleFile("101", "aks", "main.bicep");
    const target = builder.getBicepReference("aks", "v1");
    console.log(`target = ${target}`);
    invokingBicepCommand(
      "publish",
      exampleFilePath,
      "--target",
      target
    ).shouldSucceed();
  });

  it("should fail to publish invalid module", () => {
    const exampleFilePath = pathToExampleFile("101", "aks", "flawed.bicep");
    const target = builder.getBicepReference("aks-flawed", "v1");
    console.log(`target = ${target}`);
    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .shouldFail()
      .withNonEmptyStderr();
  });
});
