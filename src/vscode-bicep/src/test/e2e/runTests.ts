// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as cp from "child_process";
import * as os from "os";
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
    const userDataArgument = `--user-data-dir="${userDataDir}"`;

    const isRoot = os.userInfo().username == "root";

    const extensionInstallArguments = [
      "--install-extension",
      "ms-dotnettools.vscode-dotnet-runtime",
    ];

    if (isRoot) {
      extensionInstallArguments.push(userDataArgument);
    }

    // Install .NET Install Tool as a dependency.
    cp.spawnSync(cliPath, extensionInstallArguments, {
      encoding: "utf-8",
      stdio: "inherit",
    });

    await runTests({
      vscodeExecutablePath,
      extensionDevelopmentPath: path.resolve(__dirname, "../../.."),
      extensionTestsPath: path.resolve(__dirname, "index"),
      launchArgs: isRoot
        ? ["--enable-proposed-api", userDataArgument]
        : ["--enable-proposed-api"],
    });

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
