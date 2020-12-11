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
      extensionTestsEnv: { NODE_ENV: "test" },
      launchArgs: ["--enable-proposed-api", ...userDataArguments],
    });

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
