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
} from "vscode-azureextensionui";
import { ErrorAction, Message, CloseAction } from "vscode-languageclient/node";

const dotnetRuntimeVersion = "5.0";
const packagedServerPath = "bicepLanguageServer/Bicep.LangServer.dll";
const extensionId = "ms-azuretools.vscode-bicep";

export async function launchLanguageServiceWithProgressReport(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Promise<lsp.LanguageClient> {
  return await vscode.window.withProgress(
    {
      title: "Launching Bicep language service...",
      location: vscode.ProgressLocation.Notification,
    },
    async () => await launchLanguageService(context, outputChannel)
  );
}

async function launchLanguageService(
  context: vscode.ExtensionContext,
  outputChannel: vscode.OutputChannel
): Promise<lsp.LanguageClient> {
  getLogger().info("Launching Bicep language service...");

  const dotnetCommandPath = await ensureDotnetRuntimeInstalled();
  getLogger().debug(`Found dotnet command at '${dotnetCommandPath}'.`);

  const languageServerPath = ensureLanguageServerExists(context);
  getLogger().debug(`Found language server at '${languageServerPath}'.`);

  const serverExecutable: lsp.Executable = {
    command: dotnetCommandPath,
    args: [languageServerPath],
    options: {
      env: process.env,
    },
  };

  const serverOptions: lsp.ServerOptions = {
    run: serverExecutable,
    debug: serverExecutable,
  };

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
      // These file watcher globs should be kept in-sync with those defined in BicepDidChangeWatchedFilesHander.cs
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

  context.subscriptions.push(client.start());

  getLogger().info("Bicep language service started.");

  await client.onReady();

  getLogger().info("Bicep language service ready.");

  return client;
}

async function ensureDotnetRuntimeInstalled(): Promise<string> {
  getLogger().info("Acquiring dotnet runtime...");

  const result = await vscode.commands.executeCommand<{ dotnetPath: string }>(
    "dotnet.acquire",
    {
      version: dotnetRuntimeVersion,
      requestingExtensionId: extensionId,
    }
  );

  if (!result) {
    const errorMessage = `Failed to install .NET runtime v${dotnetRuntimeVersion}.`;

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
    ): ErrorAction {
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
    closed(): CloseAction {
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
