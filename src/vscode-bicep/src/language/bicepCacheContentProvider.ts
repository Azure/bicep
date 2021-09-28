// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils";
import { bicepCacheRequestType } from "./protocol";

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
      })
    );
  }

  onDidChange?: vscode.Event<vscode.Uri> | undefined;

  async provideTextDocumentContent(
    uri: vscode.Uri,
    token: vscode.CancellationToken
  ): Promise<string> {
    const response = await this.languageClient.sendRequest(
      bicepCacheRequestType,
      {
        target: this.getTarget(uri),
      },
      token
    );

    return response.content;
  }

  private getTarget(uri: vscode.Uri) {
    // the URIs have the format of bicep-cache:///<uri-encoded bicep module reference>
    // the path of a URI will also have a leading slash that needs to be removed
    return decodeURIComponent(uri.path.substring(1));
  }

  private getModuleReferenceScheme(document: vscode.TextDocument) {
    const moduleReferenceWithLeadingSeparator = document.uri.path;
    const colonIndex = moduleReferenceWithLeadingSeparator.indexOf(":");
    if (colonIndex < 0) {
      throw new Error(
        `The document URI '${document.uri.toString()}' has an unexpected format.`
      );
    }

    // skip over the leading separator
    return moduleReferenceWithLeadingSeparator.substring(1, colonIndex);
  }

  private tryFixCacheContentLanguage(document: vscode.TextDocument) {
    if (
      document.uri.scheme === "bicep-cache" &&
      document.languageId === "plaintext"
    ) {
      // the file is showing content from the bicep cache and the language is still set to plain text
      // we should try to correct it
      const scheme = this.getModuleReferenceScheme(document);
      vscode.languages.setTextDocumentLanguage(
        document,
        this.getLanguageId(scheme)
      );
    }
  }

  private getLanguageId(scheme: string) {
    switch (scheme) {
      case "ts":
        return "json";
      case "br": {
        const armToolsExtension = vscode.extensions.getExtension(
          "msazurermtools.azurerm-vscode-tools"
        );

        // if ARM Tools extension is installed and active, use a more specific language ID
        // otherwise, fall back to JSON
        return armToolsExtension && armToolsExtension.isActive
          ? "arm-template"
          : "json";
      }
      default:
        return "plaintext";
    }
  }
}
