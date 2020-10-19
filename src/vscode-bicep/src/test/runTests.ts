// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as path from "path";
import { runTests } from "vscode-test";

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
    await runTests({
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
