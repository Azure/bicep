// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { existsSync } from "fs";
import path from "path";
import { ExtensionContext, lm, McpStdioServerDefinition } from "vscode";
import { Disposable } from "../../infrastructure/lifecycle";

const packagedMcpServerPath = "bicepMcpServer/Azure.Bicep.McpServer.dll";

export function activateMcpFeature(
  extension: Disposable,
  extensionContext: ExtensionContext,
  dotnetCommandPath: string,
): void {
  extension.register(
    lm.registerMcpServerDefinitionProvider("bicep", {
      provideMcpServerDefinitions: async () => {
        const mcpServerPath = ensureMcpServerExists(extensionContext);
        return [new McpStdioServerDefinition("Bicep", dotnetCommandPath, [mcpServerPath])];
      },
    }),
  );
}

function ensureMcpServerExists(context: ExtensionContext): string {
  const mcpServerPath = process.env.BICEP_MCP_SERVER_PATH ?? context.asAbsolutePath(packagedMcpServerPath);

  if (!existsSync(mcpServerPath)) {
    throw new Error(`MCP server does not exist at '${mcpServerPath}'.`);
  }

  return path.resolve(mcpServerPath);
}
