// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { spawnSync } from "child_process";
import path from "path";
import { fileURLToPath } from "url";

export const extensionRoot = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "..");
export const uiRoot = path.resolve(extensionRoot, "../vscode-bicep-ui");

export function runCommand(name, command, args, cwd) {
  console.log(`\n==> ${name}`);
  const result = spawnSync(command, args, {
    cwd,
    stdio: "inherit",
    shell: process.platform === "win32" && command === "npm",
  });

  if (result.error) {
    throw result.error;
  }
  if (result.status !== 0) {
    throw new Error(`${name} failed with exit code ${result.status ?? "unknown"}.`);
  }
}

export function setupDevelopment() {
  runCommand("Install VS Code UI dependencies", "npm", ["install"], uiRoot);
  runCommand("Build VS Code UI", "npm", ["run", "build"], uiRoot);
  runCommand("Install VS Code extension dependencies", "npm", ["install"], extensionRoot);
  runCommand(
    "Build Bicep language server",
    "dotnet",
    ["build", "../Bicep.LangServer/Bicep.LangServer.csproj"],
    extensionRoot,
  );
  runCommand("Build Bicep MCP server", "dotnet", ["build", "../Bicep.McpServer/Bicep.McpServer.csproj"], extensionRoot);
}

if (process.argv[1] && path.resolve(process.argv[1]) === fileURLToPath(import.meta.url)) {
  setupDevelopment();
}
