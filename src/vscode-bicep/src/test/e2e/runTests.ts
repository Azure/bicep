// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as cp from "child_process";
import * as os from "os";
import * as fs from "fs";
import { minVersion } from "semver";
import {
  runTests,
  downloadAndUnzipVSCode,
  resolveCliPathFromVSCodeExecutablePath,
} from "vscode-test";

async function go() {
  try {
    // Do not import the json file directly because it's not under /src.
    // We also don't want it to be included in the /out folder.
    const packageJsonPath = path.resolve(__dirname, "../../../package.json");
    const packageJson = JSON.parse(
      fs.readFileSync(packageJsonPath, { encoding: "utf-8" })
    );
    const minSupportedVSCodeSemver = minVersion(packageJson.engines.vscode);

    if (!minSupportedVSCodeSemver) {
      throw new Error(
        "Ensure 'engines.vscode' is properly set in package.json"
      );
    }

    const vscodeVersionsToVerify = [minSupportedVSCodeSemver.version, "stable"];

    for (const vscodeVersion of vscodeVersionsToVerify) {
      console.log(`Running tests against VSCode-${vscodeVersion}`);

      const vscodeExecutablePath = await downloadAndUnzipVSCode(vscodeVersion);
      const cliPath =
        resolveCliPathFromVSCodeExecutablePath(vscodeExecutablePath);

      const isRoot = os.userInfo().username === "root";

      // some of our builds run as root in a container, which requires passing
      // the user data folder relative path to vs code itself
      const userDataDir = "./.vscode-test/user-data";
      const userDataArguments = isRoot ? ["--user-data-dir", userDataDir] : [];

      const extensionInstallArguments = [
        "--install-extension",
        "ms-dotnettools.vscode-dotnet-runtime",
        ...userDataArguments,
      ];

      // Install .NET Install Tool as a dependency.
      cp.spawnSync(cliPath, extensionInstallArguments, {
        encoding: "utf-8",
        stdio: "inherit",
      });

      await runTests({
        vscodeExecutablePath,
        extensionDevelopmentPath: path.resolve(__dirname, "../../.."),
        extensionTestsPath: path.resolve(__dirname, "index"),
        extensionTestsEnv: { TEST_MODE: "e2e" },
        launchArgs: ["--enable-proposed-api", ...userDataArguments],
      });
    }

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
