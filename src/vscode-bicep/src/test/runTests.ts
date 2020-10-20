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
  const args = process.argv.slice(2);

  if (args.length === 0) {
    console.error("Expect a test type parameter.");
    process.exit(1);
  }

  const testType = args[0];

  if (testType !== "e2e" && testType !== "unit") {
    console.error(
      `Invalid test type: "${testType}". Valid test types are: "e2e", "unit".`
    );
    process.exit(1);
  }

  try {
    const vscodeExecutablePath = await downloadAndUnzipVSCode("stable");
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
      extensionDevelopmentPath: path.resolve(__dirname, "../.."),
      extensionTestsPath: path.resolve(__dirname, testType, "index"),
      launchArgs: ["--enable-proposed-api"],
    });
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

go();
