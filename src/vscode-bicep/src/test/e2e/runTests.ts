// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import * as cp from "child_process";
import {
  runTests,
  downloadAndUnzipVSCode,
  resolveCliPathFromVSCodeExecutablePath,
} from "vscode-test";

async function go() {
  try {
    const vscodeExecutablePath = await downloadAndUnzipVSCode("1.46.1");
    const cliPath = resolveCliPathFromVSCodeExecutablePath(
      vscodeExecutablePath
    );

    // Install .NET Install Tool as a dependency.
    cp.spawnSync(
      cliPath,
      ["--install-extension", "ms-dotnettools.vscode-dotnet-runtime"],
      { encoding: "utf-8", stdio: "inherit" }
    );

    await runTests({
      vscodeExecutablePath,
      extensionDevelopmentPath: path.resolve(__dirname, "../../.."),
      extensionTestsPath: path.resolve(__dirname, "index"),
      launchArgs: ["--enable-proposed-api"],
    });

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
