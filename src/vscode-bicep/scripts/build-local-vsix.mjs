// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { cpSync, rmSync } from "fs";
import path from "path";
import { extensionRoot, runCommand, setupDevelopment } from "./setup-development.mjs";

setupDevelopment();
copyServerOutput("Bicep language server", "../Bicep.LangServer/bin/Debug/net10.0", "bicepLanguageServer");
copyServerOutput("Bicep MCP server", "../Bicep.McpServer/bin/Debug/net10.0", "bicepMcpServer");
runCommand("Build local VSIX", "npm", ["run", "package"], extensionRoot);

function copyServerOutput(name, sourcePath, destinationPath) {
  console.log(`\n==> Copy ${name}`);
  const source = path.resolve(extensionRoot, sourcePath);
  const destination = path.resolve(extensionRoot, destinationPath);
  rmSync(destination, { force: true, recursive: true });
  cpSync(source, destination, { recursive: true });
}
