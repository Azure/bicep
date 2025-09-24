// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { BicepRegistryReferenceBuilder } from "../utils/br";
import { invokingBicepCommand } from "../utils/command";
import { copyToTempFile, pathToExampleFile } from "../utils/fs";
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

    publishExtension(target).shouldSucceed().withEmptyStdout();

    invokingBicepCommand("local-deploy", files.bicepparam)
      .shouldSucceed()
      .withStdoutContaining('sayHiResult │ Hello, World!', true);
  });
});
