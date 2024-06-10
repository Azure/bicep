// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/**
 * Tests for "bicep local-deploy".
 *
 * @group CI
 */

import spawn from "cross-spawn";
import os from "os";
import path from "path";
import { invokingBicepCommand } from "./utils/command";
import {
  ensureParentDirExists,
  expectFileExists,
  pathToExampleFile,
  pathToTempFile,
} from "./utils/fs";

const itif = (condition: boolean) => condition ? it : it.skip;
const cliDotnetRid = process.env.BICEP_CLI_DOTNET_RID;
// We don't have an easy way of running this test for linux-musl-x64 RID, so skip for now.
const canRunLocalDeploy = () => !cliDotnetRid || architectures.map(x => x.dotnetRid).includes(cliDotnetRid);

const mockExtensionExeName = 'bicep-ext-mock';
const mockExtensionProjPath = path.resolve(
  __dirname,
    "../../Bicep.Local.Extension.Mock"
);

const architectures = [
  { dotnetRid: 'osx-arm64', bicepArgs: ['--bin-osx-arm64', `${mockExtensionProjPath}/bin/release/net8.0/osx-arm64/publish/${mockExtensionExeName}`] },
  { dotnetRid: 'osx-x64', bicepArgs: ['--bin-osx-x64', `${mockExtensionProjPath}/bin/release/net8.0/osx-x64/publish/${mockExtensionExeName}`] },
  { dotnetRid: 'linux-x64', bicepArgs: ['--bin-linux-x64', `${mockExtensionProjPath}/bin/release/net8.0/linux-x64/publish/${mockExtensionExeName}`] },
  { dotnetRid: 'win-x64', bicepArgs: ['--bin-win-x64', `${mockExtensionProjPath}/bin/release/net8.0/win-x64/publish/${mockExtensionExeName}.exe`] },
];

describe("bicep local-deploy", () => {
  itif(canRunLocalDeploy())("should build and deploy a provider published to the local file system", () => {

    for (const arch of architectures) {
      execDotnet(['publish', '--verbosity', 'quiet', '--configuration', 'release', '--self-contained', 'true', '-r', arch.dotnetRid, mockExtensionProjPath]);
    }

    const typesIndexPath = pathToTempFile("local-deploy", "types", "index.json");
    const typesDir = path.dirname(typesIndexPath);
    ensureParentDirExists(typesIndexPath);

    execDotnet(['run', '--verbosity', 'quiet', '--project', mockExtensionProjPath], {
      MOCK_TYPES_OUTPUT_PATH: typesDir,
    });

    const targetPath = pathToTempFile("local-deploy", "provider.tgz");

    invokingBicepCommand(
      "publish-provider",
      typesIndexPath,
      "--target",
      targetPath,
      ...(architectures.flatMap(arch => arch.bicepArgs))
    )
      .shouldSucceed()
      .withEmptyStdout();

    expectFileExists(targetPath);

    const bicepparamFilePath = pathToExampleFile("local-deploy", "main.bicepparam");

    invokingBicepCommand("local-deploy", bicepparamFilePath)
      .shouldSucceed()
      .withStdout([
        'Output sayHiResult: "Hello, World!"',
        'Resource sayHi (Create): Succeeded',
        'Result: Succeeded',
        ''
      ].join(os.EOL));
  });
});

function execDotnet(args: string[], envOverrides?: NodeJS.ProcessEnv) {
  const result = spawn.sync('dotnet', args, {
    encoding: 'utf-8',
    stdio: 'inherit',
    env: {
      ...process.env,
      ...(envOverrides ?? {})
    }
  });
  expect(result.status).toBe(0);
}