// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import * as path from "path";
import * as vscode from "vscode";
import { Uri } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { BicepExternalSourceScheme, decodeExternalSourceUri } from "./decodeExternalSourceUri";
import { BicepExternalSourceParams, bicepExternalSourceRequestType } from "./protocol";

export class BicepExternalSourceContentProvider extends Disposable implements vscode.TextDocumentContentProvider {
  constructor(private readonly languageClient: LanguageClient) {
    super();
    this.register(
      vscode.workspace.onDidOpenTextDocument((document) => {
        /*
         * Changing the language ID while the file is being opened causes one of the following problems:
         * - getting a TextDocument and blocking on it causes a deadlock
         * - doing the same in a fire/forget promise causes strange caching behavior in VS code where
         *   the language server is called for a particular file only once
         * Moving this to an event listener instead avoids these issues entirely.
         */
        this.trySetExternalSourceLanguage(document);
      }),
    );
  }

  onDidChange?: vscode.Event<vscode.Uri> | undefined;

  async provideTextDocumentContent(uri: vscode.Uri, token: vscode.CancellationToken): Promise<string> {
    // Ask the language server for the sources for the cached module
    const response = await this.languageClient.sendRequest(
      bicepExternalSourceRequestType,
      this.bicepExternalSourceRequest(uri),
      token,
    );

    return response.error ? `// ${response.error}` : (response.content ?? "");
  }

  private bicepExternalSourceRequest(uri: vscode.Uri): BicepExternalSourceParams {
    const { moduleReference, requestedSourceFile } = decodeExternalSourceUri(uri);
    return {
      target: moduleReference,
      requestedSourceFile,
    };
  }

  private getModuleReferenceScheme(uri: Uri): "br" | "ts" {
    // e.g. 'br:registry.azurecr.io/module:v3' => 'br'
    const { moduleReference } = decodeExternalSourceUri(uri);

    const colonIndex = moduleReference.indexOf(":");
    if (colonIndex >= 0) {
      const scheme = moduleReference.substring(0, colonIndex);
      if (scheme === "br" || scheme === "ts") {
        return scheme;
      }
    }

    throw new Error(`The document URI '${uri.toString()}' is in an unexpected format.`);
  }

  private trySetExternalSourceLanguage(document: vscode.TextDocument): void {
    if (document.uri.scheme === BicepExternalSourceScheme && document.languageId === "plaintext") {
      // The file is showing content from the bicep cache and the language is still set to plain text, so
      // we should try to correct it

      const scheme = this.getModuleReferenceScheme(document.uri);
      const { requestedSourceFile } = decodeExternalSourceUri(document.uri);

      // Not necessary to wait for this to finish
      void vscode.languages.setTextDocumentLanguage(
        document,
        // If no requestedSourceFile, we're being asked for the compiled main.json file
        this.getLanguageId(scheme, requestedSourceFile ?? "main.json"),
      );
    }
  }

  private getLanguageId(scheme: "br" | "ts", fileName: string) {
    switch (scheme) {
      case "ts":
        return "json";
      case "br": {
        if (path.extname(fileName) === ".bicep") {
          return "bicep";
        }

        const armToolsExtension = vscode.extensions.getExtension("msazurermtools.azurerm-vscode-tools");

        // if ARM Tools extension is installed and active, use a more specific language ID
        // otherwise, fall back to JSON
        return armToolsExtension && armToolsExtension.isActive ? "arm-template" : "jsonc";
      }
      default:
        return "plaintext";
    }
  }
}
