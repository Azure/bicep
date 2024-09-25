// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Live tests for "bicep local-deploy".
 *
 * @group CI
 */

import spawn from "cross-spawn";
import path from "path";
import { ensureParentDirExists } from "./fs";
import { invokingBicepCommand } from "./command";

const mockExtensionExeName = "bicep-ext-mock";
const mockExtensionProjPath = path.resolve(
  __dirname,
  "../../../Bicep.Local.Extension.Mock",
);

function execDotnet(args: string[], envOverrides?: NodeJS.ProcessEnv) {
  const result = spawn.sync("dotnet", args, {
    encoding: "utf-8",
    stdio: "inherit",
    env: {
      ...process.env,
      ...(envOverrides ?? {}),
    },
  });

  expect(result.status).toBe(0);
}

type ExtensionConfiguration = {
  dotnetRid: string;
  dotnetPublishPath: string;
  bicepCliPublishArg: string;
};

const supportedConfigurations: ExtensionConfiguration[] = [
  {
    dotnetRid: "osx-arm64",
    bicepCliPublishArg: "--bin-osx-arm64",
    dotnetPublishPath: `${mockExtensionProjPath}/bin/release/net8.0/osx-arm64/publish/${mockExtensionExeName}`,
  },
  {
    dotnetRid: "osx-x64",
    bicepCliPublishArg: "--bin-osx-x64",
    dotnetPublishPath: `${mockExtensionProjPath}/bin/release/net8.0/osx-x64/publish/${mockExtensionExeName}`,
  },
  {
    dotnetRid: "linux-x64",
    bicepCliPublishArg: "--bin-linux-x64",
    dotnetPublishPath: `${mockExtensionProjPath}/bin/release/net8.0/linux-x64/publish/${mockExtensionExeName}`,
  },
  {
    dotnetRid: "win-x64",
    bicepCliPublishArg: "--bin-win-x64",
    dotnetPublishPath: `${mockExtensionProjPath}/bin/release/net8.0/win-x64/publish/${mockExtensionExeName}.exe`,
  },
];

export function platformSupportsLocalDeploy() {
  const cliDotnetRid = process.env.BICEP_CLI_DOTNET_RID;

  // We don't have an easy way of running this test for linux-musl-x64 RID, so skip for now.
  return (
    !cliDotnetRid ||
    supportedConfigurations.map((x) => x.dotnetRid).includes(cliDotnetRid)
  );
}

export function publishExtension(typesIndexPath: string, target: string) {
  // build the binary in different flavors
  for (const config of supportedConfigurations) {
    execDotnet([
      "publish",
      "--verbosity",
      "quiet",
      "--configuration",
      "release",
      "--self-contained",
      "true",
      "-r",
      config.dotnetRid,
      mockExtensionProjPath,
    ]);
  }

  const typesDir = path.dirname(typesIndexPath);
  ensureParentDirExists(typesIndexPath);

  // generate types on disk
  execDotnet(
    ["run", "--verbosity", "quiet", "--project", mockExtensionProjPath],
    {
      MOCK_TYPES_OUTPUT_PATH: typesDir,
    },
  );

  // run the bicep command to publish it
  return invokingBicepCommand(
    "publish-extension",
    typesIndexPath,
    "--target",
    target,
    ...supportedConfigurations.flatMap((c) => [
      c.bicepCliPublishArg,
      c.dotnetPublishPath,
    ]),
  );
}
