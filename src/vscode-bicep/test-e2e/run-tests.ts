// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as cp from "child_process";
import * as fs from "fs";
import * as os from "os";
import * as path from "path";
import { downloadAndUnzipVSCode, resolveCliArgsFromVSCodeExecutablePath, runTests } from "@vscode/test-electron";
import { minVersion } from "semver";

async function go() {
  try {
    // This script runs in regular Node.js. It launches VS Code, which then loads the compiled
    // test-e2e/index module inside the extension host and calls its exported run() function.
    const useLocalServers = process.argv.includes("--local");

    // Do not import the json file directly because it's not under /src.
    // We also don't want it to be included in the /out folder.
    const packageJsonPath = path.resolve(__dirname, "../../package.json");
    const extensionDevelopmentPath = path.dirname(packageJsonPath);
    const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, { encoding: "utf-8" }));
    const minSupportedVSCodeSemver = minVersion(packageJson.engines.vscode);

    if (!minSupportedVSCodeSemver) {
      throw new Error("Ensure 'engines.vscode' is properly set in package.json");
    }

    const vscodeVersionsToVerify = [minSupportedVSCodeSemver.version, "stable"];

    for (const vscodeVersion of vscodeVersionsToVerify) {
      console.log(`Running tests against VSCode-${vscodeVersion}`);

      const vscodeExecutablePath = await downloadAndUnzipVSCode(vscodeVersion);
      const [cliRawPath, ...cliArguments] = resolveCliArgsFromVSCodeExecutablePath(vscodeExecutablePath);
      const cliPath = `"${cliRawPath}"`;

      const isRoot = os.userInfo().username === "root";

      // some of our builds run as root in a container, which requires passing
      // the user data folder relative path to vs code itself
      const userDataDir = "./.vscode-test/user-data";
      const userDataArguments = isRoot ? ["--user-data-dir", userDataDir] : [];

      const extensionInstallArguments = [
        ...cliArguments,
        "--install-extension",
        "ms-dotnettools.vscode-dotnet-runtime",
        ...userDataArguments,
      ];
      const extensionListArguments = [...cliArguments, "--list-extensions", ...userDataArguments];

      // Install .NET Install Tool extension as a dependency.
      console.log(`Installing dotnet extension: ${cliPath} ${extensionInstallArguments.join(" ")}`);
      let result = cp.spawnSync(cliPath, extensionInstallArguments, {
        encoding: "utf-8",
        stdio: "inherit",
        shell: true,
      });
      console.log(result.error ?? result.output?.filter((o) => !!o).join("\n"));

      console.log("Installed extensions:");
      result = cp.spawnSync(cliPath, extensionListArguments, {
        encoding: "utf-8",
        stdio: "inherit",
        shell: true,
      });
      console.log(result.error ?? result.output?.filter((o) => !!o).join("\n"));
      if (result.error) {
        process.exit(1);
      }

      await runTests({
        vscodeExecutablePath,
        extensionDevelopmentPath,
        extensionTestsPath: path.resolve(__dirname, "index"),
        extensionTestsEnv: {
          TEST_MODE: "e2e",
          ...(useLocalServers ? getLocalServerEnvironment(extensionDevelopmentPath) : {}),
        },
        launchArgs: [
          "--no-sandbox",
          "--disable-gpu-sandbox",
          "--enable-proposed-api=ms-azuretools.vscode-bicep",
          ...userDataArguments,
        ],
      });
    }

    process.exit(0);
  } catch (err) {
    console.error(err);
    process.exit(1);
  }
}

function getLocalServerEnvironment(extensionPath: string): NodeJS.ProcessEnv {
  return {
    BICEP_LANGUAGE_SERVER_PATH: requireFile(
      path.resolve(extensionPath, "../Bicep.LangServer/bin/Debug/net10.0/Bicep.LangServer.dll"),
      "dotnet build ../Bicep.LangServer/Bicep.LangServer.csproj",
    ),
    BICEP_MCP_SERVER_PATH: requireFile(
      path.resolve(extensionPath, "../Bicep.McpServer/bin/Debug/net10.0/Azure.Bicep.McpServer.dll"),
      "dotnet build ../Bicep.McpServer/Bicep.McpServer.csproj",
    ),
  };
}

function requireFile(filePath: string, buildCommand: string): string {
  if (!fs.existsSync(filePath)) {
    throw new Error(`Required local server does not exist at '${filePath}'. Run: ${buildCommand}`);
  }

  return filePath;
}

void go();
