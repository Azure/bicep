// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import {
  LanguageClient,
  TextDocumentIdentifier,
} from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { bicepCacheRequestType } from "./protocol";
import * as path from "path";
import { Uri } from "vscode";

export class BicepCacheContentProvider
  extends Disposable
  implements vscode.TextDocumentContentProvider
{
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
        this.tryFixCacheContentLanguage(document);
      }),
    );
  }

  onDidChange?: vscode.Event<vscode.Uri> | undefined;

  async provideTextDocumentContent(
    uri: vscode.Uri,
    token: vscode.CancellationToken,
  ): Promise<string> {
    // Ask the language server for the sources for the cached module
    const response = await this.languageClient.sendRequest(
      bicepCacheRequestType,
      this.getBicepCacheRequest(uri),
      token,
    );

    return response.content;
  }

  private getBicepCacheRequest(uri: vscode.Uri) {
    const [moduleReference, cachePath] = this.decodeBicepCacheUri(uri);
    return {
      textDocument: TextDocumentIdentifier.create(cachePath),
      target: moduleReference,
    };
  }

  private decodeBicepCacheUri(uri: vscode.Uri): [
    moduleReference: string, // e.g. br:myregistry.azurecr.io/myrepo:v1
    cachePath: string, // eg /Users/MyUserName/.bicep/br/myregistry.azurecr.io/myrepo/v1$/main.json
  ] {
    // The uri passed in has this format:
    //   bicep-cache:module-reference#cache-file-path
    //
    // Example decoded URI:
    //   bicep-cache:br:myregistry.azurecr.io/myrepo:v1#/Users/MyUserName/.bicep/br/registry.azurecr.io/myrepo/v1$/main.json
    const registry = decodeURIComponent(uri.path);
    const cachePath = decodeURIComponent(uri.fragment);

    return [registry, cachePath];
  }

  private getModuleReferenceScheme(uri: Uri): "br" | "ts" {
    // e.g. 'br:registry.azurecr.io/module:v3'
    const [moduleReference] = this.decodeBicepCacheUri(uri);

    const colonIndex = moduleReference.indexOf(":");
    if (colonIndex >= 0) {
      const scheme = moduleReference.substring(0, colonIndex);
      if (scheme === "br" || scheme === "ts") {
        return scheme;
      }
    }

    throw new Error(
      `The document URI '${uri.toString()}' is in an unexpected format.`,
    );
  }

  private tryFixCacheContentLanguage(document: vscode.TextDocument): void {
    if (
      document.uri.scheme === "bicep-cache" &&
      document.languageId === "plaintext"
    ) {
      // the file is showing content from the bicep cache and the language is still set to plain text
      // we should try to correct it

      const scheme = this.getModuleReferenceScheme(document.uri);
      const [, cachePath] = this.decodeBicepCacheUri(document.uri);

      // Not necessary to wait for this to finish
      void vscode.languages.setTextDocumentLanguage(
        document,
        this.getLanguageId(scheme, cachePath),
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

        const armToolsExtension = vscode.extensions.getExtension(
          "msazurermtools.azurerm-vscode-tools",
        );

        // if ARM Tools extension is installed and active, use a more specific language ID
        // otherwise, fall back to JSON
        return armToolsExtension && armToolsExtension.isActive
          ? "arm-template"
          : "jsonc";
      }
      default:
        return "plaintext";
    }
  }
}
