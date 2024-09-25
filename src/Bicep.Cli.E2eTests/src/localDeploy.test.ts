// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep local-deploy".
 *
 * @group CI
 */

import os from "os";
import { invokingBicepCommand } from "./utils/command";
import { copyToTempFile, pathToExampleFile, pathToTempFile } from "./utils/fs";
import {
  platformSupportsLocalDeploy,
  publishExtension,
} from "./utils/localdeploy";
import { itif } from "./utils/testHelpers";

describe("bicep local-deploy", () => {
  const testArea = "local-deploy";

  // eslint-disable-next-line jest/require-hook
  itif(platformSupportsLocalDeploy())(
    "should publish and run an extension published to the local file system",
    () => {
      const baseFolder = pathToExampleFile("local-deploy");
      const target = pathToTempFile(testArea, "mock.tgz");

      const files = {
        bicep: copyToTempFile(baseFolder, "main.bicep", testArea),
        bicepparam: copyToTempFile(baseFolder, "main.bicepparam", testArea),
        bicepconfig: copyToTempFile(baseFolder, `bicepconfig.json`, testArea, {
          relativePath: "bicepconfig.json",
          values: {
            $TARGET_REFERENCE: "./mock.tgz",
          },
        }),
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
