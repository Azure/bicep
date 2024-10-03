// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep local-deploy".
 *
 * @group live
 */

import os from "os";
import { invokingBicepCommand } from "./utils/command";
import { copyToTempFile, pathToExampleFile, pathToTempFile } from "./utils/fs";
import {
  platformSupportsLocalDeploy,
  publishExtension,
} from "./utils/localdeploy";
import { getEnvironment } from "./utils/liveTestEnvironments";
import { BicepRegistryReferenceBuilder } from "./utils/br";
import { itif } from "./utils/testHelpers";

describe("bicep local-deploy", () => {
  const environment = getEnvironment();
  const testArea = `local-deploy-live${environment.suffix}`;
  const builder = new BicepRegistryReferenceBuilder(
    environment.registryUri,
    testArea,
  );

  // eslint-disable-next-line jest/require-hook
  itif(platformSupportsLocalDeploy())(
    "should publish and run an extension published to a registry",
    () => {
      const baseFolder = pathToExampleFile("local-deploy");
      const target = builder.getBicepReference("mock", "0.0.1");

      const files = {
        bicep: copyToTempFile(baseFolder, "main.bicep", testArea),
        bicepparam: copyToTempFile(baseFolder, "main.bicepparam", testArea),
        bicepconfig: copyToTempFile(
          baseFolder,
          `bicepconfig${environment.suffix}.json`,
          testArea,
          {
            relativePath: "bicepconfig.json",
            values: {
              $TARGET_REFERENCE: target,
            },
          },
        ),
      };

      const typesIndexPath = pathToTempFile(testArea, "types", "index.json");
      publishExtension(typesIndexPath, target)
        .shouldSucceed()
        .withEmptyStdout();

      invokingBicepCommand("local-deploy", files.bicepparam)
        .shouldSucceed()
        .withStdout(
          [
            'Output sayHiResult: "Hello, World!"',
            "Resource sayHi (Create): Succeeded",
            "Result: Succeeded",
            "",
          ].join(os.EOL),
        );
    },
  );
});
