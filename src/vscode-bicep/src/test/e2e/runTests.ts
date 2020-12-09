// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as cp from "child_process";
import * as fs from "fs";
import {
  runTests,
  downloadAndUnzipVSCode,
  resolveCliPathFromVSCodeExecutablePath,
} from "vscode-test";

async function go() {
  try {
    const vscodeExecutablePath = await downloadAndUnzipVSCode("stable");
    const cliPath = resolveCliPathFromVSCodeExecutablePath(
      vscodeExecutablePath
    );

    // some of our builds run as root in a container, which requires passing
    // the user data folder path to vs code itself
    const userDataDir = path.resolve(__dirname, "user-data");

    fs.mkdirSync(userDataDir, { recursive: true });

    // Install .NET Install Tool as a dependency.
    cp.spawnSync(
      cliPath,
      [
        "--install-extension",
        "ms-dotnettools.vscode-dotnet-runtime",
        `--user-data-dir="${userDataDir}"`,
      ],
      { encoding: "utf-8", stdio: "inherit" }
    );

    await runTests({
      vscodeExecutablePath,
      extensionDevelopmentPath: path.resolve(__dirname, "../../.."),
      extensionTestsPath: path.resolve(__dirname, "index"),
      launchArgs: ["--enable-proposed-api", `--user-data-dir="${userDataDir}"`],
    });

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
