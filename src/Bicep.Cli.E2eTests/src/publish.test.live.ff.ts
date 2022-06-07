// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep publish".
 *
 * @group live/ff
 */

import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { pathToExampleFile } from "./utils/fs";
import {
  fairfax,
  fairfaxEnvironmentOverrides,
} from "./utils/liveTestEnvironments";

describe("bicep publish", () => {
  const testArea = "publish";
  const builder = new BicepRegistryReferenceBuilder(
    fairfax.registryUri,
    testArea
  );

  it("should publish valid module", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + fairfax.suffix,
      "main.bicep"
    );
    const target = builder.getBicepReference("aks", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(fairfaxEnvironmentOverrides)
      .shouldSucceed();
  });

  it("should publish valid module with alias", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + fairfax.suffix,
      "main.bicep"
    );
    const target = builder.getBicepReferenceWithAlias(
      "publish-alias",
      "aks",
      "v1"
    );

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(fairfaxEnvironmentOverrides)
      .shouldSucceed();
  });

  it("should fail to publish invalid module", () => {
    const exampleFilePath = pathToExampleFile(
      "101",
      "aks" + fairfax.suffix,
      "flawed.bicep"
    );
    const target = builder.getBicepReference("aks-flawed", "v1");

    invokingBicepCommand("publish", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(fairfaxEnvironmentOverrides)
      .shouldFail()
      .withNonEmptyStderr();
  });
});
