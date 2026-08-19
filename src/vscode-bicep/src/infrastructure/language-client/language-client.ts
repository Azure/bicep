// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { existsSync } from "fs";
import * as path from "path";
import * as vscode from "vscode";
import * as lsp from "vscode-languageclient/node";
import { Message, TransportKind } from "vscode-languageclient/node";
import { bicepLanguageId } from "../editor";
import { getLogger } from "../logging";
import { Telemetry } from "../telemetry";

const dotnetRuntimeVersion = "10.0";
const packagedServerPath = "bicepLanguageServer/Bicep.LangServer.dll";
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
  outputChannel: vscode.LogOutputChannel,
  dotnetCommandPath: string,
  telemetry: Telemetry,
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

  configureTelemetry(client, telemetry);

  // To enable language server tracing, you MUST have a package setting named 'bicep.trace.server'; I was unable to find a way to enable it through code.
  // See https://github.com/microsoft/vscode-languageserver-node/blob/77c3a10a051ac619e4e3ef62a3865717702b64a3/client/src/common/client.ts#L3268

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

export async function ensureDotnetRuntimeInstalled(): Promise<string> {
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

interface LanguageServerTelemetryRule {
  properties?: readonly string[];
  measurements?: readonly string[];
}

const languageServerTelemetryRules: Record<string, LanguageServerTelemetryRule> = {
  "decompile/success": { measurements: ["countOutputFiles", "countConflictingFiles"] },
  "decompile/failure": {},
  "decompileSave/success": {},
  "decompileSave/failure": { properties: ["failureType"] },
  decompileForPaste: {
    properties: ["pasteContext", "pasteType", "languageId"],
    measurements: ["jsonSize", "bicepSize"],
  },
  "InsertResource/success": { properties: ["resourceType", "apiVersion"] },
  "InsertResource/failure": { properties: ["failureType"] },
  "ImportKubernetesManifest/success": {},
  "ImportKubernetesManifest/failure": { properties: ["failureType"] },
  "deploy/result": { properties: ["result"] },
  "ExternalSourceRequest/success": {
    properties: ["hasSource", "fileExtension", "requestType"],
    measurements: ["archiveFilesCount"],
  },
  "ExternalSourceRequest/failure": { properties: ["failureType"] },
  unhandledException: {},
};

const languageServerErrorEvents = new Set([
  "decompile/failure",
  "decompileSave/failure",
  "InsertResource/failure",
  "ImportKubernetesManifest/failure",
  "ExternalSourceRequest/failure",
  "unhandledException",
]);

function configureTelemetry(client: lsp.LanguageClient, telemetry: Telemetry) {
  const startTime = Date.now();
  const defaultErrorHandler = client.createDefaultErrorHandler();

  client.onTelemetry((telemetryData: { eventName: string; properties: { [key: string]: string | undefined } }) => {
    sendLanguageServerTelemetry(telemetry, telemetryData);
  });

  client.clientOptions.errorHandler = {
    error(error: Error, message: Message | undefined, count: number | undefined) {
      telemetry.sendError(
        "bicep.lsp-error",
        error,
        undefined,
        { secondsSinceStart: (Date.now() - startTime) / 1000 },
      );
      return defaultErrorHandler.error(error, message, count);
    },
    closed() {
      telemetry.sendError(
        "bicep.lsp-error",
        undefined,
        { reason: "closed" },
        { secondsSinceStart: (Date.now() - startTime) / 1000 },
      );
      return defaultErrorHandler.closed();
    },
  };
}

function sendLanguageServerTelemetry(
  telemetry: Telemetry,
  telemetryData: { eventName: string; properties: Record<string, string | undefined> },
): void {
  const rule = languageServerTelemetryRules[telemetryData.eventName];
  if (!rule) {
    return;
  }

  const properties: Record<string, string> = {};
  for (const propertyName of rule.properties ?? []) {
    const value = telemetryData.properties[propertyName];
    if (value) {
      properties[propertyName] = value;
    }
  }

  const measurements: Record<string, number> = {};
  for (const measurementName of rule.measurements ?? []) {
    const value = Number(telemetryData.properties[measurementName]);
    if (Number.isFinite(value)) {
      measurements[measurementName] = value;
    }
  }

  const isError =
    languageServerErrorEvents.has(telemetryData.eventName) ||
    (telemetryData.eventName === "deploy/result" && telemetryData.properties.result !== "Succeeded");
  const eventProperties = Object.keys(properties).length > 0 ? properties : undefined;
  const eventMeasurements = Object.keys(measurements).length > 0 ? measurements : undefined;

  if (isError) {
    telemetry.sendError(telemetryData.eventName, undefined, eventProperties, eventMeasurements);
  } else {
    telemetry.sendEvent(telemetryData.eventName, eventProperties, eventMeasurements);
  }
}
