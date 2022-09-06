// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import * as lsp from "vscode-languageclient/node";
import * as path from "path";
import { existsSync } from "fs";
import { getLogger } from "../utils/logger";
import {
  callWithTelemetryAndErrorHandlingSync,
  IActionContext,
  parseError,
} from "@microsoft/vscode-azext-utils";
import {
  ErrorHandlerResult,
  Message,
  CloseHandlerResult,
  TransportKind,
} from "vscode-languageclient/node";
import { writeDeploymentOutputMessageToBicepOperationsOutputChannel } from "../commands/deployHelper";

const dotnetRuntimeVersion = "6.0";
const packagedServerPath = "bicepLanguageServer/Bicep.LangServer.dll";
const extensionId = "ms-azuretools.vscode-bicep";

function getServerStartupOptions(
  dotnetCommandPath: string,
  languageServerPath: string,
  transportKind: TransportKind,
  waitForDebugger: boolean
): lsp.ServerOptions {
  const args = [];
  if (waitForDebugger) {
    // pause language server startup until a dotnet debugger has been attached
    args.push(`--wait-for-debugger`);
  }
  const envVars = {
    ...process.env,
    ...getFeatureEnvVars(),
  };

  switch (transportKind) {
    case TransportKind.stdio: {
      const executable = {
        command: dotnetCommandPath,
        args: [languageServerPath, ...args],
        options: {
          env: envVars,
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
          env: envVars,
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
  actionContext: IActionContext,
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Promise<lsp.LanguageClient> {
  getLogger().info("Launching Bicep language service...");

  const dotnetCommandPath = await ensureDotnetRuntimeInstalled(actionContext);
  getLogger().debug(`Found dotnet command at '${dotnetCommandPath}'.`);

  const languageServerPath = ensureLanguageServerExists(context);
  getLogger().debug(`Found language server at '${languageServerPath}'.`);

  const serverOptions = getServerStartupOptions(
    dotnetCommandPath,
    languageServerPath,
    // Use named pipe transport for LSP comms
    TransportKind.pipe,
    // Set to true to pause server startup until a dotnet debugger is attached
    false
  );

  const clientOptions: lsp.LanguageClientOptions = {
    documentSelector: [{ language: "bicep" }],
    initializationOptions: {
      // this tells the server that this client can handle additional DocumentUri schemes
      enableRegistryContent: true,
    },
    progressOnInitialization: true,
    outputChannel,
    middleware: {
      provideDocumentFormattingEdits: (document, options, token, next) =>
        next(
          document,
          {
            ...options,
            insertFinalNewline:
              vscode.workspace
                .getConfiguration("files")
                .get("insertFinalNewline") ?? false,
          },
          token
        ),
    },
    synchronize: {
      // These file watcher globs should be kept in-sync with those defined in BicepDidChangeWatchedFilesHandler.cs
      fileEvents: [
        vscode.workspace.createFileSystemWatcher("**/"), // folder changes
        vscode.workspace.createFileSystemWatcher("**/*.bicep"), // .bicep file changes
        vscode.workspace.createFileSystemWatcher("**/*.{json,jsonc,arm}"), // ARM template file changes
      ],
    },
  };

  const client = new lsp.LanguageClient(
    "bicep",
    "Bicep",
    serverOptions,
    clientOptions
  );

  client.registerProposedFeatures();

  configureTelemetry(client);

  // To enable language server tracing, you MUST have a package setting named 'bicep.trace.server'; I was unable to find a way to enable it through code.
  // See https://github.com/microsoft/vscode-languageserver-node/blob/77c3a10a051ac619e4e3ef62a3865717702b64a3/client/src/common/client.ts#L3268

  client.onNotification(
    "deploymentComplete",
    writeDeploymentOutputMessageToBicepOperationsOutputChannel
  );

  client.onNotification("bicep/triggerEditorCompletion", () => {
    vscode.commands.executeCommand("editor.action.triggerSuggest");
  });

  return client;
}

async function ensureDotnetRuntimeInstalled(
  actionContext: IActionContext
): Promise<string> {
  getLogger().info("Acquiring dotnet runtime...");

  const result = await vscode.commands.executeCommand<{ dotnetPath: string }>(
    "dotnet.acquire",
    {
      version: dotnetRuntimeVersion,
      requestingExtensionId: extensionId,
    }
  );

  if (!result) {
    // Suppress the 'Report Issue' button - we want people to use the dialog displayed by the .NET installer extension.
    // It captures much more detail about the problem, and directs people to the correct repo (https://github.com/dotnet/vscode-dotnet-runtime).
    actionContext.errorHandling.suppressReportIssue = true;
    const errorMessage = `Failed to install .NET runtime v${dotnetRuntimeVersion}. Please see the .NET install tool error dialog for more detailed information, or to report an issue.`;

    getLogger().error(errorMessage);
    throw new Error(errorMessage);
  }

  return path.resolve(result.dotnetPath);
}

function ensureLanguageServerExists(context: vscode.ExtensionContext): string {
  const languageServerPath =
    process.env.BICEP_LANGUAGE_SERVER_PATH ?? // Local server for debugging.
    context.asAbsolutePath(packagedServerPath); // Packaged server.

  if (!existsSync(languageServerPath)) {
    throw new Error(
      `Language server does not exist at '${languageServerPath}'.`
    );
  }

  return path.resolve(languageServerPath);
}

function configureTelemetry(client: lsp.LanguageClient) {
  const startTime = Date.now();
  const defaultErrorHandler = client.createDefaultErrorHandler();

  client.onTelemetry(
    (telemetryData: {
      eventName: string;
      properties: { [key: string]: string | undefined };
    }) => {
      callWithTelemetryAndErrorHandlingSync(
        telemetryData.eventName,
        (telemetryActionContext) => {
          telemetryActionContext.errorHandling.suppressDisplay = true;
          telemetryActionContext.telemetry.properties =
            telemetryData.properties;
        }
      );
    }
  );

  client.clientOptions.errorHandler = {
    error(
      error: Error,
      message: Message | undefined,
      count: number | undefined
    ): ErrorHandlerResult {
      callWithTelemetryAndErrorHandlingSync(
        "bicep.lsp-error",
        (context: IActionContext) => {
          context.telemetry.properties.jsonrpcMessage = message
            ? message.jsonrpc
            : "";
          context.telemetry.measurements.secondsSinceStart =
            (Date.now() - startTime) / 1000;

          throw new Error(`Error: ${parseError(error).message}`);
        }
      );
      return defaultErrorHandler.error(error, message, count);
    },
    closed(): CloseHandlerResult {
      callWithTelemetryAndErrorHandlingSync(
        "bicep.lsp-error",
        (context: IActionContext) => {
          context.telemetry.measurements.secondsSinceStart =
            (Date.now() - startTime) / 1000;

          throw new Error(`Connection closed`);
        }
      );
      return defaultErrorHandler.closed();
    },
  };
}

function getFeatureEnvVars() {
  const importsEnabledExperimental = vscode.workspace
    .getConfiguration("bicep")
    .get<boolean>("importsEnabledExperimental");

  return {
    BICEP_IMPORTS_ENABLED_EXPERIMENTAL: importsEnabledExperimental,
  };
}
