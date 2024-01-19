// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep publish-provider".
 *
 * @group live
 */

import path from "path";
import { BicepRegistryReferenceBuilder } from "./utils/br";
import { invokingBicepCommand } from "./utils/command";
import { pathToExampleFile, readFileSync, writeTempFile } from "./utils/fs";
import { getEnvironment } from "./utils/liveTestEnvironments";

describe("bicep publish-provider", () => {
  const testArea = "publish-provider";
  const environment = getEnvironment();

  it("should publish provider", () => {
    const builder = new BicepRegistryReferenceBuilder(
      environment.registryUri,
      testArea
    );
    const baseFolder = pathToExampleFile("providers" + environment.suffix);
    const indexJsonPath = path.join(baseFolder, "types/http/index.json");
    const target = builder.getBicepProviderReference("http", "0.0.1");

    // This should no longer be necessary once https://github.com/Azure/bicep/issues/12202 has been implemented.
    const publishTarget = target.replace('@', ':');

    invokingBicepCommand("publish-provider", indexJsonPath, "--target", publishTarget)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed();
  });

  it("should be consumable and result in a valid compilation", () => {
    const builder = new BicepRegistryReferenceBuilder(
      environment.registryUri,
      testArea
    );
    const baseFolder = pathToExampleFile("providers" + environment.suffix);
    const indexJsonPath = path.join(baseFolder, "types/http/index.json");
    const target = builder.getBicepProviderReference("http", "0.0.1");

    // This should no longer be necessary once https://github.com/Azure/bicep/issues/12202 has been implemented.
    const publishTarget = target.replace('@', ':');

    invokingBicepCommand("publish-provider", indexJsonPath, "--target", publishTarget)
      .withEnvironmentOverrides(environment.environmentOverrides)
      .shouldSucceed();

    const bicepContents = readFileSync(
      path.join(baseFolder, "main.bicep")
    ).replace('$TARGET_REFERENCE', target);
    const bicepPath = writeTempFile("restore-provider", "main.bicep", bicepContents);

    const configContents = readFileSync(
      path.join(baseFolder, "bicepconfig.json")
    );
    writeTempFile("restore-provider", "bicepconfig.json", configContents);

    // Building with --stdout should emit a valid result.
    invokingBicepCommand("build", "--stdout", bicepPath)
      .shouldSucceed()
      .withNonEmptyStdout();
  });
});
