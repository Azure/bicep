// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import crypto from "crypto";
import { getResourceCatalogRequestType, ResourceTypeCatalog } from "../../language/protocol";
import { LanguageClient } from "vscode-languageclient/node";

export class DraggableResourcesViewProvider
  implements vscode.WebviewViewProvider
{
  private readonly extensionUri: vscode.Uri;
  private readonly rootUri: vscode.Uri;
  private resourceCatalog: ResourceTypeCatalog;

  constructor(extensionUri: vscode.Uri, private readonly languageClient: LanguageClient) {
    this.extensionUri = vscode.Uri.joinPath(extensionUri, "..", "vscode-bicep-ui");
    this.rootUri = vscode.Uri.joinPath(
      this.extensionUri,
      "apps",
      "resource-type-explorer",
      "dist",
    );
    this.resourceCatalog = [];
  }

  resolveWebviewView(webviewView: vscode.WebviewView) {
    webviewView.webview.options = {
      enableScripts: true,
      localResourceRoots: [this.rootUri],
    };

    // TODO: Update to send textDocument to get current features to find what resources are available instead of loading all resources.
    webviewView.webview.html = this.createWebviewHtml(webviewView.webview);

    webviewView.webview.onDidReceiveMessage(async (message) => {
      switch (message.method) {
        case "resourceTypeCatalog/load":
          if (this.resourceCatalog.length == 0) {
            const resourceCatalog = await this.languageClient.sendRequest(getResourceCatalogRequestType, {});
            this.resourceCatalog = resourceCatalog;
          }
          await webviewView.webview.postMessage({id: message.id, result: this.resourceCatalog});
          break;
      }
    });
  }

  private getWebviewResourceUri(
    webview: vscode.Webview,
    ...resourceName: string[]
  ) {
    return webview.asWebviewUri(
      vscode.Uri.joinPath(this.rootUri, ...resourceName),
    );
  }

  private createWebviewHtml(webview: vscode.Webview): string {
    const { cspSource } = webview;
    const nonce = crypto.randomBytes(16).toString("hex");
    const stylesUri = this.getWebviewResourceUri(
      webview,
      "assets",
      "index.css",
    );
    const scriptUri = this.getWebviewResourceUri(webview, "index.js");

    return `
      <!DOCTYPE html>
      <html lang="en">
      <head>
        <meta charset="UTF-8">
        <!--
        Use a content security policy to only allow loading images from our extension directory,
        and only allow scripts that have a specific nonce.
        -->
        <meta http-equiv="Content-Security-Policy" content="default-src 'none'; font-src ${cspSource} data:; style-src ${cspSource} 'unsafe-inline'; img-src ${cspSource} data:; script-src 'unsafe-inline' vscode-webview-resource:;">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link rel="stylesheet" type="text/css" href="${stylesUri}">
      </head>
      <body>
        <div id="root"></div>
        <script type="module" nonce="${nonce}" src="${scriptUri}" />
      </body>
      </html>`;
  }
}
