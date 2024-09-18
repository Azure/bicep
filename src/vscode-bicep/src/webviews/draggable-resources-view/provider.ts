// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";
import crypto from "crypto";

export class DraggableResourcesViewProvider
  implements vscode.WebviewViewProvider
{
  private readonly extensionUri: vscode.Uri;
  private readonly rootUri: vscode.Uri;

  constructor(extensionUri: vscode.Uri) {
    this.extensionUri = vscode.Uri.joinPath(extensionUri, "..", "vscode-bicep-ui");
    this.rootUri = vscode.Uri.joinPath(
      this.extensionUri,
      "apps",
      "resource-type-explorer",
      "dist",
    );
  }

  resolveWebviewView(webviewView: vscode.WebviewView) {
    webviewView.webview.options = {
      enableScripts: true,
      localResourceRoots: [this.rootUri],
    };
    webviewView.webview.html = this.createWebviewHtml(webviewView.webview);
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
        <script>
          const resourceTypeCatalog = [
          {
            group: "Microsoft.Compute",
            resourceTypes: [
              {
                resourceType: "availabilitySets",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachines",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachines/extensions",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachineScaleSets",
                apiVersion: "2022-09-01"
              },
              {
                resourceType:"virtualMachineScaleSets/virtualMachines",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachineScaleSets/virtualMachines/extensions",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachineScaleSets/networkInterfaces",
                apiVersion: "2022-09-01"
              },
              {
                resourceType:  "virtualMachineScaleSets/virtualMachines/networkInterfaces",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachineScaleSets/publicIPAddresses",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/operations",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/vmSizes",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/runCommands",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/systemInfo",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/virtualMachines",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/virtualMachineScaleSets",
                apiVersion: "2022-09-01"
              },
              {
                resourceType:"locations/publishers",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "operations",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachines/runCommands",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "virtualMachineScaleSets/applications",
                apiVersion: "2022-09-01"
              },
              {
                resourceType:  "virtualMachines/VMApplications",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/edgeZones",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/edgeZones/vmimages",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/edgeZones/publishers",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "restorePointCollections",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "restorePointCollections/restorePoints",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "proximityPlacementGroups",
                apiVersion: "2022-09-01"
              }
            ],
          },
          {
            group: "Microsoft.Web",
            resourceTypes: [
            {
                resourceType:"publishingUsers",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "ishostnameavailable",
                apiVersion: "2022-09-01"
              },
              {
                resourceType:  "validate",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "isusernameavailable",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "generateGithubAccessTokenForAppserviceCLI",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "sourceControls",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "availableStacks",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "webAppStacks",
                apiVersion: "2022-09-01"
              },
              {
                resourceType: "locations/webAppStacks",
                apiVersion: "2022-09-01"
              }
            ],
          },
        ];
        const acquireVsCodeApi = () => ({
          postMessage: ({ id, method }) => {
            if (method === "resourceTypeCatalog/load") {
              window.postMessage({ id, result: resourceTypeCatalog });
            } else {
              window.postMessage({ id, error: "Invalid request." });
            }
          },
          getState: () => {},
          setState: () => {},
        });

        window.acquireVsCodeApi = acquireVsCodeApi;
        </script>
        <script type="module" nonce="${nonce}" src="${scriptUri}" />
      </body>
      </html>`;
  }
}
