// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep publish-provider".
 *
 * @group live
 */

import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { pathToExampleFile } from "./utils/fs";
import { getEnvironment } from "./utils/liveTestEnvironments";

describe("bicep publish-provider", () => {
  const testArea = "publish";
  const environment = getEnvironment();
  const builder = new BicepRegistryReferenceBuilder(
    environment.registryUri,
    testArea
  );

  it("should publish provider", () => {
    const exampleFilePath = pathToExampleFile(
      "providers" + environment.suffix,
      "index.json"
    );
    const target = builder.getBicepReference("providers", "v1");

    invokingBicepCommand("publish-provider", exampleFilePath, "--target", target)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed();
  });
});
