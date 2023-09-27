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
    this.extensionUri = extensionUri;
    this.rootUri = vscode.Uri.joinPath(
      this.extensionUri,
      "ui",
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
          const resourceTypeCatalog = {
            "Microsoft.Compute": [
              "availabilitySets",
              "virtualMachines",
              "virtualMachines/extensions",
              "virtualMachineScaleSets",
              "virtualMachineScaleSets/extensions",
              "virtualMachineScaleSets/virtualMachines",
              "virtualMachineScaleSets/virtualMachines/extensions",
              "virtualMachineScaleSets/networkInterfaces",
              "virtualMachineScaleSets/virtualMachines/networkInterfaces",
              "virtualMachineScaleSets/publicIPAddresses",
              "locations",
              "locations/operations",
              "locations/vmSizes",
              "locations/runCommands",
              "locations/systemInfo",
              "locations/virtualMachines",
              "locations/virtualMachineScaleSets",
              "locations/publishers",
              "operations",
              "virtualMachines/runCommands",
              "virtualMachineScaleSets/applications",
              "virtualMachines/VMApplications",
              "locations/edgeZones",
              "locations/edgeZones/vmimages",
              "locations/edgeZones/publishers",
              "restorePointCollections",
              "restorePointCollections/restorePoints",
              "proximityPlacementGroups",
              "sshPublicKeys",
              "capacityReservationGroups",
              "capacityReservationGroups/capacityReservations",
              "virtualMachines/metricDefinitions",
              "locations/spotEvictionRates",
              "locations/spotPriceHistory",
              "locations/recommendations",
              "locations/sharedGalleries",
              "locations/communityGalleries",
              "sharedVMImages",
              "sharedVMImages/versions",
              "locations/artifactPublishers",
              "locations/capsoperations",
              "galleries",
              "galleries/images",
              "galleries/images/versions",
              "locations/galleries",
              "galleries/applications",
              "galleries/applications/versions",
              "sharedVMExtensions",
              "sharedVMExtensions/versions",
              "galleries/serviceArtifacts",
              "disks",
              "snapshots",
              "locations/diskoperations",
              "diskEncryptionSets",
              "diskAccesses",
              "restorePointCollections/restorePoints/diskRestorePoints",
              "virtualMachineScaleSets/disks",
              "locations/vsmoperations",
              "cloudServices",
              "cloudServices/roles",
              "cloudServices/roleInstances",
              "locations/csoperations",
              "locations/cloudServiceOsVersions",
              "locations/cloudServiceOsFamilies",
              "cloudServices/networkInterfaces",
              "cloudServices/roleInstances/networkInterfaces",
              "cloudServices/publicIPAddresses",
              "locations/usages",
              "images",
              "locations/diagnostics",
              "locations/diagnosticOperations",
              "locations/logAnalytics",
              "hostGroups",
              "hostGroups/hosts",
            ],
            "Microsoft.Web": [
              "publishingUsers",
              "ishostnameavailable",
              "validate",
              "isusernameavailable",
              "generateGithubAccessTokenForAppserviceCLI",
              "sourceControls",
              "availableStacks",
              "webAppStacks",
              "locations/webAppStacks",
              "functionAppStacks",
              "locations/functionAppStacks",
              "staticSites",
              "locations/previewStaticSiteWorkflowFile",
              "staticSites/userProvidedFunctionApps",
              "staticSites/linkedBackends",
              "staticSites/builds/linkedBackends",
              "staticSites/databaseConnections",
              "staticSites/builds/databaseConnections",
              "staticSites/builds",
              "staticSites/builds/userProvidedFunctionApps",
              "freeTrialStaticWebApps",
              "listSitesAssignedToHostName",
              "locations/getNetworkPolicies",
              "locations/operations",
              "locations/operationResults",
              "sites/networkConfig",
              "sites/slots/networkConfig",
              "sites/hostNameBindings",
              "sites/slots/hostNameBindings",
              "operations",
              "certificates",
              "serverFarms",
              "sites",
              "sites/slots",
              "runtimes",
              "recommendations",
              "resourceHealthMetadata",
              "georegions",
              "sites/premieraddons",
              "hostingEnvironments",
              "hostingEnvironments/multiRolePools",
              "hostingEnvironments/workerPools",
              "kubeEnvironments",
              "deploymentLocations",
              "deletedSites",
              "locations/deletedSites",
              "ishostingenvironmentnameavailable",
              "locations/deleteVirtualNetworkOrSubnets",
              "locations/validateDeleteVirtualNetworkOrSubnets",
              "apiManagementAccounts",
              "apiManagementAccounts/connections",
              "apiManagementAccounts/connectionAcls",
              "apiManagementAccounts/apis/connections/connectionAcls",
              "apiManagementAccounts/apis/connectionAcls",
              "apiManagementAccounts/apiAcls",
              "apiManagementAccounts/apis/apiAcls",
              "apiManagementAccounts/apis",
              "apiManagementAccounts/apis/localizedDefinitions",
              "apiManagementAccounts/apis/connections",
              "connections",
              "customApis",
              "locations",
              "locations/listWsdlInterfaces",
              "locations/extractApiDefinitionFromWsdl",
              "locations/managedApis",
              "locations/runtimes",
              "locations/apiOperations",
              "connectionGateways",
              "locations/connectionGatewayInstallations",
              "checkNameAvailability",
              "billingMeters",
              "verifyHostingEnvironmentVnet",
              "serverFarms/eventGridFilters",
              "sites/eventGridFilters",
              "sites/slots/eventGridFilters",
              "hostingEnvironments/eventGridFilters",
              "serverFarms/firstPartyApps",
              "serverFarms/firstPartyApps/keyVaultSettings",
              "workerApps",
              "containerApps",
              "customhostnameSites",
            ],
          };

          const acquireVsCodeApi = () => ({
            postMessage: ({ requestId, kind }) => {
              if (kind === "resourceTypeCatalog/load") {
                window.postMessage({
                  requestId,
                  response: resourceTypeCatalog,
                });
              } else {
                window.postMessage({
                  requestId,
                  error: "Invalid request kind.",
                });
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
