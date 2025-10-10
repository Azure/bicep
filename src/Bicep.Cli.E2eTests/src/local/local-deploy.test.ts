// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { describe, it } from "vitest";
import { invokingBicepCommand } from "../utils/command";
import { copyToTempFile, pathToExampleFile, pathToTempFile } from "../utils/fs";
import { platformSupportsLocalDeploy, publishExtension } from "../utils/localdeploy";

describe("bicep local-deploy", () => {
  const testArea = "local-deploy";

  it.runIf(platformSupportsLocalDeploy())(
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

      publishExtension(target).shouldSucceed().withEmptyStdout();

      invokingBicepCommand("local-deploy", files.bicepparam)
        .shouldSucceed()
        .withStdoutContaining("sayHiResult │ Hello, World!", true);
    },
  );
});
