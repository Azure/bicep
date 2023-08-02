// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/* eslint-disable jest/expect-expect */

import vscode, { ConfigurationTarget, Selection, TextDocument } from "vscode";
import {
  executeCloseAllEditors,
  executeEditorPasteCommand,
  executePasteAsBicepCommand,
} from "./commands";
import { getBicepConfiguration } from "../../language/getBicepConfiguration";
import { normalizeMultilineString } from "../utils/normalizeMultilineString";
import { SuppressedWarningsManager } from "../../commands/SuppressedWarningsManager";
import { bicepConfigurationKeys } from "../../language/constants";
import assert from "assert";
import { until } from "../utils/time";
import * as fse from "fs-extra";
import * as path from "path";
import { e2eLogName } from "../../utils/logger";

const extensionLogPath = path.join(__dirname, `../../../${e2eLogName}`);

describe("pasteAsBicep", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  async function configureSettings(): Promise<void> {
    // Make sure Decompile on Paste is on
    await getBicepConfiguration().update(
      bicepConfigurationKeys.decompileOnPaste,
      true,
      ConfigurationTarget.Global,
    );

    // Make sure decompile on paste warning is on
    await getBicepConfiguration().update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      [],
      ConfigurationTarget.Global,
    );
  }

  function getTextAndMarkers(
    s: string,
  ): [text: string, markerOffset: number, markerLength: number] {
    const offset = s.indexOf("|");
    assert(offset >= 0, "Couldn't find marker in text");

    s = s.slice(0, offset) + s.slice(offset + 1);

    let length = 0;
    const offset2 = s.indexOf("|");
    if (offset2 >= 0) {
      length = offset2 - offset;
      s = s.slice(0, offset2) + s.slice(offset2 + 1);
    }

    expect(s).not.toContain("|");

    return [s, offset, length];
  }

  function setSelection(
    document: TextDocument,
    offsetStart: number,
    offsetLength: number,
  ): void {
    const start = document.positionAt(offsetStart);
    const end = document.positionAt(offsetStart + offsetLength);
    const activeTextEditor = vscode.window.activeTextEditor;
    assert(activeTextEditor, "No active text editor");
    activeTextEditor.selection = new Selection(start, end);
  }

  async function runTest(
    initialBicepWithMarker: string,
    jsonToPaste: string,
    action: "command" | "copy/paste",
    expected: {
      bicep?: string;
      error?: string;
    },
  ): Promise<{ log: string }> {
    const initialLogContentsLength = fse
      .readFileSync(extensionLogPath)
      .toString().length;

    await configureSettings();

    const [initialBicep, offsetStart, offsetLength] = getTextAndMarkers(
      initialBicepWithMarker,
    );
    const textDocument = await vscode.workspace.openTextDocument({
      language: "bicep",
      content: initialBicep,
    });
    const editor = await vscode.window.showTextDocument(textDocument);

    setSelection(textDocument, offsetStart, offsetLength);

    await vscode.env.clipboard.writeText(jsonToPaste);

    if (action === "copy/paste") {
      await executeEditorPasteCommand();

      const expected = `PasteAsBicep (command): Result: "${jsonToPaste}"`;
      await waitForPasteAsBicep(expected);
    } else {
      await executePasteAsBicepCommand(editor.document.uri);

      const expected = `PasteAsBicep (copy/paste): Result: "${jsonToPaste}"`;
      await waitForPasteAsBicep(expected);
    }
    if (expected.error) {
      const match = new RegExp(
        `Exception occurred: .*${escapeRegexReplacement(expected.error)}`,
      );
      expect(getRecentLogContents()).toMatch(match);
    } else {
      expect(getRecentLogContents()).not.toMatch(`Exception occurred`);
    }

    const buffer = textDocument.getText();

    if (typeof expected.bicep === "string") {
      expect(normalizeMultilineString(buffer)).toBe(
        normalizeMultilineString(expected.bicep),
      );
    }

    return { log: getRecentLogContents() };

    function getRecentLogContents() {
      const logContents = fse
        .readFileSync(extensionLogPath)
        .toString()
        .substring(initialLogContentsLength);
      return logContents;
    }

    async function waitForPasteAsBicep(
      expectedSubstring: string,
    ): Promise<void> {
      await until(() => isReady(), {
        interval: 100,
        timeoutMs: 4000,
      });
      if (!isReady()) {
        throw new Error(
          `Expected paste as bicep command to complete. Expected following string in log: "${expectedSubstring}".\nRecent log contents: ${getRecentLogContents()}`,
        );
      }

      function isReady(): boolean {
        const readyMessage = jsonToPaste;
        const logContents = getRecentLogContents();
        return logContents.indexOf(readyMessage) >= 0;
      }
    }
  }

  function escapeRegexReplacement(s: string) {
    return s.replace(/\$/g, "$$$$");
  }

  //////////////////////////////////////////////////

  const fullTemplate1 = `
  {
      "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
      "contentVersion": "1.0.0.0",
      "parameters": {
          "location": {
              "type": "string"
          },
          "existingVirtualMachineNames": {
              "type": "array"
          },
          "sqlServerLicenseType": {
              "type": "string"
          },
          "existingVmResourceGroup": {
              "type": "string"
          },
          "groupResourceId": {
              "type": "string"
          },
          "domainAccountPassword": {
              "type": "securestring"
          },
          "sqlServicePassword": {
              "type": "securestring"
          }
      },
      "variables": {
      },
      "resources": [
          {
              "name": "[trim(parameters('existingVirtualMachineNames')[copyIndex()])]",
              "type": "Microsoft.SqlVirtualMachine/SqlVirtualMachines",
              "apiVersion": "2017-03-01-preview",
              "location": "[parameters('location')]",
              "copy": {
                  "name": "vmToClusterLoop",
                  "count": "[length(parameters('existingVirtualMachineNames'))]"
              },
              "properties": {
                  "virtualMachineResourceId": "[resourceId(parameters('existingVmResourceGroup'),'Microsoft.Compute/virtualMachines', trim(parameters('existingVirtualMachineNames')[copyIndex()]))]",
                  "sqlServerLicenseType": "[parameters('sqlServerLicenseType')]",
                  "SqlVirtualMachineGroupResourceId": "[parameters('groupResourceId')]",
                  "WSFCDomainCredentials": {
                      "ClusterBootstrapAccountPassword": "[parameters('domainAccountPassword')]",
                      "ClusterOperatorAccountPassword": "[parameters('domainAccountPassword')]",
                      "SqlServiceAccountPassword": "[parameters('sqlServicePassword')]"
                  }
              }
          }
      ]
  }`;

  const fullTemplateExpectedBicep = `param location string
param existingVirtualMachineNames array
param sqlServerLicenseType string
param existingVmResourceGroup string
param groupResourceId string

@secure()
param domainAccountPassword string

@secure()
param sqlServicePassword string

resource existingVirtualMachineNames_resource 'Microsoft.SqlVirtualMachine/SqlVirtualMachines@2017-03-01-preview' = [for item in existingVirtualMachineNames: {
  name: trim(item)
  location: location
  properties: {
    virtualMachineResourceId: resourceId(existingVmResourceGroup, 'Microsoft.Compute/virtualMachines', trim(item))
    sqlServerLicenseType: sqlServerLicenseType
    sqlVirtualMachineGroupResourceId: groupResourceId
    wsfcDomainCredentials: {
      clusterBootstrapAccountPassword: domainAccountPassword
      clusterOperatorAccountPassword: domainAccountPassword
      sqlServiceAccountPassword: sqlServicePassword
    }
  }
}]`;

  it("should convert pasted full ARM template - copy/paste", async () => {
    await runTest(`|`, fullTemplate1, "copy/paste", {
      bicep: fullTemplateExpectedBicep,
    });
  });

  it("should convert pasted full ARM template - paste command", async () => {
    await runTest(`|`, fullTemplate1, "command", {
      bicep: fullTemplateExpectedBicep,
    });
  });

  it("should convert pasted list of resources", async () => {
    const jsonToPaste = `
    {
      "type": "Microsoft.Storage/storageAccounts",
        "apiVersion": "2021-02-01",
          "name": "stg1",
            "location": "[parameters('location2')]",
              "kind": "StorageV2",
                "sku": {
        "name": "Premium_LRS"
      }
    } /* no comma */{
      "name": "aksCluster1",
        "type": "Microsoft.ContainerService/managedClusters",
          "apiVersion": "2021-05-01",
            "location": "[resourceGroup().location]",
              "properties": {
        "kubernetesVersion": "1.15.7",
          "dnsPrefix": "dnsprefix",
            "agentPoolProfiles": [
              {
                "name": "agentpool",
                "count": 2,
                "vmSize": "Standard_A1",
                "osType": "Linux",
                "storageProfile": "ManagedDisks"
              }
            ],
              "linuxProfile": {
          "adminUsername": "adminUserName",
            "ssh": {
            "publicKeys": [
              {
                "keyData": "keyData"
              }
            ]
          }
        },
        "servicePrincipalProfile": {
          "clientId": "servicePrincipalAppId",
            "secret": "servicePrincipalAppPassword"
        }
      }
    }
  ,`;

    const expected = `resource stg1 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'stg1'
  location: location2
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}

resource aksCluster1 'Microsoft.ContainerService/managedClusters@2021-05-01' = {
  name: 'aksCluster1'
  location: resourceGroup().location
  properties: {
    kubernetesVersion: '1.15.7'
    dnsPrefix: 'dnsprefix'
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: 2
        vmSize: 'Standard_A1'
        osType: 'Linux'
        storageProfile: 'ManagedDisks'
      }
    ]
    linuxProfile: {
      adminUsername: 'adminUserName'
      ssh: {
        publicKeys: [
          {
            keyData: 'keyData'
          }
        ]
      }
    }
    servicePrincipalProfile: {
      clientId: 'servicePrincipalAppId'
      secret: 'servicePrincipalAppPassword'
    }
  }
}
// My bicep file
      `;

    await runTest(`|// My bicep file\n`, jsonToPaste, "copy/paste", {
      bicep: expected,
    });
  });

  //////////////////////////////////////////////////

  it("should decompile if copy/pasting outside string", async () => {
    const bicep = `var v = |`;
    const jsonToPaste = `"Mom says 'hi'"`;
    const expected = `var v = 'Mom says \\'hi\\''`;

    await runTest(bicep, jsonToPaste, "copy/paste", { bicep: expected });
  });

  it("should not decompile if copy/pasting inside string", async () => {
    const bicep = `@description('|this is a description')
param s string`;
    const jsonToPaste = `"Mom says 'hi' "`;
    const expected = `@description('"Mom says 'hi' "this is a description')
param s string`;

    await runTest(bicep, jsonToPaste, "copy/paste", { bicep: expected });
  });

  it("should not decompile if pasting inside multiline string", async () => {
    const bicep = `var v = '''
These are
|multiple
lines'''`;
    const jsonToPaste = `"really" `;
    const expected = `var v = '''
These are
"really" multiple
lines'''`;

    await runTest(bicep, jsonToPaste, "copy/paste", { bicep: expected });
  });

  it("should not decompile even from menu command when inside string", async () => {
    const bicep = `@description('|this is a description')
param s string`;
    const jsonToPaste = `"Mom says 'hi' "`;

    await runTest(bicep, jsonToPaste, "command", {
      error: "Cannot paste JSON as Bicep inside of a string",
    });
  });

  it("should not decompile even from menu command when inside property string", async () => {
    const bicep = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  location: '|location'
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;
    const jsonToPaste = `{
      "type": "Microsoft.Resources/resourceGroups",
      "apiVersion": "2022-09-01",
      "name": "rg",
      "location": "[parameters('location')]"
    }`;

    await runTest(bicep, jsonToPaste, "command", {
      error: "Cannot paste JSON as Bicep inside of a string",
    });
  });

  it("should handle non-empty selection outside string", async () => {
    const bicep = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  location: |'location'|
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;
    const jsonToPaste = `"hello"`;
    const expected = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  location: 'hello'
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;

    await runTest(bicep, jsonToPaste, "command", { bicep: expected });
  });

  it("should handle non-empty selection inside string", async () => {
    const bicep = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  location: '|location|'
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;
    const jsonToPaste = `"hello"`;

    await runTest(bicep, jsonToPaste, "command", {
      error: "Cannot paste JSON as Bicep inside of a string",
    });
  });

  it("should handle non-empty selection inside and outside of a string (using context of first part of selection)", async () => {
    const bicep = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  |location: 'location|'
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;
    const jsonToPaste = `"hello"`;
    const expected = `resource loadBalancerPublicIPAddress 'Microsoft.Network/publicIPAddresses@2020-11-01' = {
  name: 'loadBalancerName'
  'hello''
  sku: {
    name: 'Standard'
  }
  properties: {
    publicIPAllocationMethod: 'static'
  }
}
`;

    await runTest(bicep, jsonToPaste, "command", { bicep: expected });
  });
});
