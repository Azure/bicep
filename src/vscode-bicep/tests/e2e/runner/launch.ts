// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as cp from "child_process";
import * as fs from "fs";
import * as os from "os";
import * as path from "path";
import { fileURLToPath } from "url";
import { downloadAndUnzipVSCode, resolveCliArgsFromVSCodeExecutablePath, runTests } from "@vscode/test-electron";
import { minVersion } from "semver";

async function go() {
  try {
    // This script runs in regular Node.js. It launches VS Code, which then loads the compiled
    // extension-host module inside the extension host and calls its exported run() function.
    const useLocalServers = process.argv.includes("--local");

    // Do not import the json file directly because it's not under /src.
    // We also don't want it to be included in the /out folder.
    const moduleDirectory = path.dirname(fileURLToPath(import.meta.url));
    const packageJsonPath = path.resolve(moduleDirectory, "../../../../package.json");
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

      // Network-isolated builds cannot reach the Marketplace or the dotnet CDN, so they provide a
      // pre-downloaded VSIX and a pre-installed dotnet instead. Both overrides force the relative
      // user data folder so the CLI and the test run share the settings written below.
      const dotnetExtensionVsixPath = process.env.BICEP_DOTNET_EXTENSION_VSIX_PATH;
      const dotnetRuntimePath = process.env.BICEP_DOTNET_RUNTIME_PATH;
      const userDataArguments = isRoot || dotnetRuntimePath ? ["--user-data-dir", userDataDir] : [];

      if (dotnetRuntimePath) {
        configureExistingDotnetPath(userDataDir, dotnetRuntimePath);
      }

      const dotnetExtensionToInstall = dotnetExtensionVsixPath
        ? requireFile(path.resolve(dotnetExtensionVsixPath), "set BICEP_DOTNET_EXTENSION_VSIX_PATH to an existing VSIX")
        : "ms-dotnettools.vscode-dotnet-runtime";

      const extensionInstallArguments = [
        ...cliArguments,
        "--install-extension",
        `"${dotnetExtensionToInstall}"`,
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
      if (result.error || result.status !== 0) {
        throw new Error(
          `Failed to install '${dotnetExtensionToInstall}'. The Bicep extension declares it as an extension dependency, so the E2E tests cannot run without it.`,
        );
      }

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
        extensionTestsPath: path.resolve(moduleDirectory, "extension-host.js"),
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

// Points the .NET Install Tool at an already installed dotnet so it does not download one at test
// time. The Bicep extension reads this setting through the 'dotnet.acquire' command it invokes.
function configureExistingDotnetPath(userDataDir: string, dotnetRuntimePath: string): void {
  const resolvedDotnetPath = requireFile(
    path.resolve(dotnetRuntimePath),
    "set BICEP_DOTNET_RUNTIME_PATH to an existing dotnet executable",
  );

  const settingsPath = path.resolve(userDataDir, "User/settings.json");
  fs.mkdirSync(path.dirname(settingsPath), { recursive: true });

  const settings = fs.existsSync(settingsPath) ? JSON.parse(fs.readFileSync(settingsPath, { encoding: "utf-8" })) : {};

  settings["dotnetAcquisitionExtension.existingDotnetPath"] = [
    { extensionId: "ms-azuretools.vscode-bicep", path: resolvedDotnetPath },
  ];

  fs.writeFileSync(settingsPath, JSON.stringify(settings, null, 2));
  console.log(`Configured existing dotnet path '${resolvedDotnetPath}' in '${settingsPath}'.`);
}

function requireFile(filePath: string, remediation: string): string {
  if (!fs.existsSync(filePath)) {
    throw new Error(`Required file does not exist at '${filePath}'. Run: ${remediation}`);
  }

  return filePath;
}

void go();
