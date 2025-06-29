// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import os from "os";
import { describe, it } from "vitest";
import { BicepRegistryReferenceBuilder } from "../utils/br";
import { invokingBicepCommand } from "../utils/command";
import { copyToTempFile, pathToExampleFile, pathToTempFile } from "../utils/fs";
import { getEnvironment } from "../utils/liveTestEnvironments";
import { platformSupportsLocalDeploy, publishExtension } from "../utils/localdeploy";

describe("bicep local-deploy", () => {
  const environment = getEnvironment();
  const testArea = `local-deploy-live${environment.suffix}`;
  const builder = new BicepRegistryReferenceBuilder(environment.registryUri, testArea);

  it.runIf(platformSupportsLocalDeploy())("should publish and run an extension published to a registry", () => {
    const baseFolder = pathToExampleFile("local-deploy");
    const target = builder.getBicepReference("mock", "0.0.1");

    const files = {
      bicep: copyToTempFile(baseFolder, "main.bicep", testArea),
      bicepparam: copyToTempFile(baseFolder, "main.bicepparam", testArea),
      bicepconfig: copyToTempFile(baseFolder, `bicepconfig${environment.suffix}.json`, testArea, {
        relativePath: "bicepconfig.json",
        values: {
          $TARGET_REFERENCE: target,
        },
      }),
    };

    const typesIndexPath = pathToTempFile(testArea, "types", "index.json");
    publishExtension(typesIndexPath, target).shouldSucceed().withEmptyStdout();

    invokingBicepCommand("local-deploy", files.bicepparam)
      .shouldSucceed()
      .withStdout(
        ['Output sayHiResult: "Hello, World!"', "Resource sayHi (Create): Succeeded", "Result: Succeeded", ""].join(
          os.EOL,
        ),
      );
  });
});
