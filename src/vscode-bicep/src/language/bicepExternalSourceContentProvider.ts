// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { Disposable } from "../utils/disposable";
import { bicepExternalSourceRequestType } from "./protocol";
import * as path from "path";
import { Uri } from "vscode";

export const BicepExternalSourceScheme = "bicep-extsrc";
type ExternalSource = {
  // The title to display for the document,
  //   e.g. "br:myregistry.azurecr.io/myrepo/module/main.json:v1/main.json (module:v1)" or similar
  // VSCode will display everything after the last slash in the document's tab, and the full string
  //   on hover.
  title: string;
  // Full module reference, e.g. "myregistry.azurecr.io/myrepo/module:v1"
  moduleReference: string;
  // File being requested from the source, relative to the module root.
  //   e.g. main.bicep or mypath/module.bicep
  // This should be undefined to request the compiled JSON file (can't use "main.json" because there
  //   might actually be a source file called "main.json" in the original module sources).
  requestedSourceFile?: string;
};

export class BicepExternalSourceContentProvider
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
        this.trySetExternalSourceLanguage(document);
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
      bicepExternalSourceRequestType,
      this.bicepExternalSourceRequest(uri),
      token,
    );

    return response.content;
  }

  private bicepExternalSourceRequest(uri: vscode.Uri) {
    const { moduleReference } = this.decodeExternalSourceUri(uri);
    return {
      target: moduleReference,
    };
  }

  // NOTE: This should match the logic in BicepExternalSourceRequestHandler.GetExternalSourceLinkUri and
  // also bicep\src\Bicep.LangServer.UnitTests\BicepExternalSourceRequestHandlerTests.cs.DecodeExternalSourceUri
  private decodeExternalSourceUri(uri: vscode.Uri): ExternalSource {
    // The uri passed in has this format (encoded):
    //   bicep-extsrc:{title}?{module-reference}[#{source-file-relative-path}]
    const title = decodeURIComponent(uri.path);
    const moduleReference = decodeURIComponent(uri.query);
    let requestedSourceFile: string | undefined = decodeURIComponent(
      uri.fragment,
    );

    if (requestedSourceFile === "") {
      requestedSourceFile = undefined;
    }

    return { title, moduleReference, requestedSourceFile };
  }

  private getModuleReferenceScheme(uri: Uri): "br" | "ts" {
    // e.g. 'br:registry.azurecr.io/module:v3' => 'br'
    const { moduleReference } = this.decodeExternalSourceUri(uri);

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

  private trySetExternalSourceLanguage(document: vscode.TextDocument): void {
    if (
      document.uri.scheme === BicepExternalSourceScheme &&
      document.languageId === "plaintext"
    ) {
      // The file is showing content from the bicep cache and the language is still set to plain text, so
      // we should try to correct it

      const scheme = this.getModuleReferenceScheme(document.uri);
      const { requestedSourceFile } = this.decodeExternalSourceUri(
        document.uri,
      );

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
