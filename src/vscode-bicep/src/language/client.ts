// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { existsSync } from "fs";
import * as path from "path";
import { callWithTelemetryAndErrorHandlingSync, IActionContext, parseError } from "@microsoft/vscode-azext-utils";
import * as vscode from "vscode";
import * as lsp from "vscode-languageclient/node";
import { Message, TransportKind } from "vscode-languageclient/node";
import { writeDeploymentOutputMessageToBicepOperationsOutputChannel } from "../commands/deployHelper";
import { getLogger } from "../utils/logger";
import { bicepLanguageId } from "./constants";

const dotnetRuntimeVersion = "10.0";
const packagedServerPath = "bicepLanguageServer/Bicep.LangServer.dll";
const packagedMcpServerPath = "bicepMcpServer/Bicep.McpServer.dll";
const extensionId = "ms-azuretools.vscode-bicep";
const dotnetAcquisitionExtensionSetting = "dotnetAcquisitionExtension";
const existingDotnetPathSetting = "existingDotnetPath";

function getServerStartupOptions(
  dotnetCommandPath: string,
  languageServerPath: string,
  transportKind: TransportKind,
  waitForDebugger: boolean,
): lsp.ServerOptions {
  const args = [];
  if (waitForDebugger) {
    // pause language server startup until a dotnet debugger has been attached
    args.push(`--wait-for-debugger`);
  }

  switch (transportKind) {
    case TransportKind.stdio: {
      const executable = {
        command: dotnetCommandPath,
        args: [languageServerPath, ...args],
        options: {
          env: process.env,
        },
      };
      return {
        run: executable,
        debug: executable,
      };
    }
    case TransportKind.pipe: {
      const module = {
        runtime: dotnetCommandPath,
        module: languageServerPath,
        transport: transportKind,
        args,
        options: {
          env: process.env,
        },
      };
      return {
        run: module,
        debug: module,
      };
    }
  }

  throw new Error(`TransportKind '${transportKind}' is not supported.`);
}

export async function createLanguageService(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel,
  dotnetCommandPath: string,
): Promise<lsp.LanguageClient> {
  getLogger().info("Launching Bicep language service...");

  const languageServerPath = ensureLanguageServerExists(context);
  getLogger().debug(`Found language server at '${languageServerPath}'.`);

  const serverOptions = getServerStartupOptions(
    dotnetCommandPath,
    languageServerPath,
    // Use named pipe transport for LSP comms
    TransportKind.pipe,
    // Set to true to pause server startup until a dotnet debugger is attached
    false,
  );

  const clientOptions: lsp.LanguageClientOptions = {
    documentSelector: [{ language: bicepLanguageId }],
    initializationOptions: {
      // this tells the server that this client can handle additional DocumentUri schemes (e.g. bicep-extsrc:)
      enableRegistryContent: true,
    },
    progressOnInitialization: true,
    outputChannel,
    synchronize: {
      configurationSection: "bicep",
      fileEvents: [
        // Register to watch all files and folders, regardless of extension, because they could be referenced by load* functions.
        // We will do the filtering in the language server. This glob pattern should be kept in-sync with BicepDidChangeWatchedFilesHandler.cs.
        vscode.workspace.createFileSystemWatcher("**/*"),
      ],
    },
  };

  const client = new lsp.LanguageClient(bicepLanguageId, "Bicep", serverOptions, clientOptions);

  client.registerProposedFeatures();

  configureTelemetry(client);

  // To enable language server tracing, you MUST have a package setting named 'bicep.trace.server'; I was unable to find a way to enable it through code.
  // See https://github.com/microsoft/vscode-languageserver-node/blob/77c3a10a051ac619e4e3ef62a3865717702b64a3/client/src/common/client.ts#L3268

  client.onNotification("deploymentComplete", writeDeploymentOutputMessageToBicepOperationsOutputChannel);

  client.onNotification("bicep/triggerEditorCompletion", async () => {
    await vscode.commands.executeCommand("editor.action.triggerSuggest");
  });

  return client;
}

function getCustomDotnetRuntimePathConfig() {
  const acquireConfig = vscode.workspace
    .getConfiguration(dotnetAcquisitionExtensionSetting)
    .get(existingDotnetPathSetting);
  if (!Array.isArray(acquireConfig)) {
    return null;
  }

  return acquireConfig.filter((x) => x.extensionId === extensionId)[0];
}

export async function ensureDotnetRuntimeInstalled(actionContext: IActionContext): Promise<string> {
  getLogger().info("Acquiring dotnet runtime...");

  const customDotnetRuntimePathConfig = getCustomDotnetRuntimePathConfig();
  if (customDotnetRuntimePathConfig) {
    // This setting is a common source of issues. Add explicit logging to help with investigation.
    getLogger().info(
      `Found config for '${dotnetAcquisitionExtensionSetting}.${existingDotnetPathSetting}': ${JSON.stringify(
        customDotnetRuntimePathConfig,
      )}`,
    );
  }

  const result = await vscode.commands.executeCommand<{ dotnetPath: string }>("dotnet.acquire", {
    version: dotnetRuntimeVersion,
    requestingExtensionId: extensionId,
  });

  if (!result) {
    // Suppress the 'Report Issue' button - we want people to use the dialog displayed by the .NET installer extension.
    // It captures much more detail about the problem, and directs people to the correct repo (https://github.com/dotnet/vscode-dotnet-runtime).
    actionContext.errorHandling.suppressReportIssue = true;
    const errorMessage = `Failed to install .NET runtime v${dotnetRuntimeVersion}. Please see the .NET install tool error dialog for more detailed information, or to report an issue.`;

    getLogger().error(errorMessage);
    throw new Error(errorMessage);
  }

  const dotnetPath = path.resolve(result.dotnetPath);
  if (!existsSync(dotnetPath)) {
    // The 'dotnet.acquire' command doesn't actually verify that the dotnet path is valid, in the case
    // that the user has configured a custom path using the 'dotnetAcquisitionExtension.existingDotnetPath' setting.
    // Let's sanity check it here to help users unblock themselves.
    let errorMessage = `Failed to find dotnet executable at path '${dotnetPath}'.`;
    if (customDotnetRuntimePathConfig) {
      errorMessage += ` Please ensure the path configured for extension '${extensionId}' with setting '${dotnetAcquisitionExtensionSetting}.${existingDotnetPathSetting}' is valid.`;
    }

    throw new Error(errorMessage);
  }

  getLogger().debug(`Found dotnet command at '${dotnetPath}'.`);
  return dotnetPath;
}

function ensureLanguageServerExists(context: vscode.ExtensionContext): string {
  const languageServerPath =
    process.env.BICEP_LANGUAGE_SERVER_PATH ?? // Local server for debugging.
    context.asAbsolutePath(packagedServerPath); // Packaged server.

  if (!existsSync(languageServerPath)) {
    throw new Error(`Language server does not exist at '${languageServerPath}'.`);
  }

  return path.resolve(languageServerPath);
}

export function ensureMcpServerExists(context: vscode.ExtensionContext): string {
  const mcpServerPath =
    process.env.BICEP_MCP_SERVER_PATH ?? // Local server for debugging.
    context.asAbsolutePath(packagedMcpServerPath); // Packaged server.

  if (!existsSync(mcpServerPath)) {
    throw new Error(`MCP server does not exist at '${mcpServerPath}'.`);
  }

  return path.resolve(mcpServerPath);
}

function configureTelemetry(client: lsp.LanguageClient) {
  const startTime = Date.now();
  const defaultErrorHandler = client.createDefaultErrorHandler();

  client.onTelemetry((telemetryData: { eventName: string; properties: { [key: string]: string | undefined } }) => {
    callWithTelemetryAndErrorHandlingSync(telemetryData.eventName, (telemetryActionContext) => {
      telemetryActionContext.errorHandling.suppressDisplay = true;
      telemetryActionContext.telemetry.properties = telemetryData.properties;
    });
  });

  client.clientOptions.errorHandler = {
    error(error: Error, message: Message | undefined, count: number | undefined) {
      callWithTelemetryAndErrorHandlingSync("bicep.lsp-error", (context: IActionContext) => {
        context.telemetry.properties.jsonrpcMessage = message ? message.jsonrpc : "";
        context.telemetry.measurements.secondsSinceStart = (Date.now() - startTime) / 1000;

        throw new Error(`Error: ${parseError(error).message}`);
      });
      return defaultErrorHandler.error(error, message, count);
    },
    closed() {
      callWithTelemetryAndErrorHandlingSync("bicep.lsp-error", (context: IActionContext) => {
        context.telemetry.measurements.secondsSinceStart = (Date.now() - startTime) / 1000;

        throw new Error(`Connection closed`);
      });
      return defaultErrorHandler.closed();
    },
  };
}
