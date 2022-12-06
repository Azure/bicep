// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { ConfigurationTarget } from "vscode";
import { executeCloseAllEditors } from "./commands";
import { getBicepConfiguration } from "../../language/getBicepConfiguration";
import { until } from "../utils/time";
import { normalizeMultilineString } from "../utils/normalizeMultilineString";
import { SuppressedWarningsManager } from "../../commands/SuppressedWarningsManager";

describe("pasteAsBicep", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  async function configureSettings(): Promise<void> {
    // Make sure Decompile on Paste is on
    await getBicepConfiguration().update(
      "decompileOnPaste",
      true,
      ConfigurationTarget.Global
    );

    // Make sure decompile on paste warning is on
    await getBicepConfiguration().update(
      SuppressedWarningsManager.suppressedWarningsConfigurationKey,
      [],
      ConfigurationTarget.Global
    );
  }

  it("should convert pasted full templates", async () => {
    const json = `
{
	"$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
	"contentVersion": "1.0.0.0",
	"metadata": {
		"prefix": "arm-nested-template-inner",
		"description": "Nested (inline) Inner-Scoped Deployment. Defines its own local parameters."
	},
	"resources": [
		{
            "name": "nestedDeployment1",
            "type": "Microsoft.Resources/deployments",
            "apiVersion": "2021-04-01",
            "properties": {
                "mode": "Incremental",
                "template": {
                    "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
                    "contentVersion": "1.0.0.0",
                    "variables": {},
                    "resources": [],
                    "outputs": {}
                }
            }
        }
	]
}`;

    const waitFor = "module nestedDeployment1";
    const expected = `metadata prefix = 'arm-nested-template-inner'
metadata description = 'Nested (inline) Inner-Scoped Deployment. Defines its own local parameters.'

module nestedDeployment1 './nested_nestedDeployment1.bicep' = {
  name: 'nestedDeployment1'
  params: {
  }
}
// My bicep file`;

    await configureSettings();

    const textDocument = await vscode.workspace.openTextDocument({
      language: "bicep",
      content: "// My bicep file\n",
    });
    await vscode.window.showTextDocument(textDocument);

    vscode.env.clipboard.writeText(json);
    await vscode.commands.executeCommand("editor.action.clipboardPasteAction");

    await until(() => textDocument.getText().includes(waitFor), {
      timeoutMs: 10000,
    });
    const buffer = textDocument.getText();

    expect(normalizeMultilineString(buffer)).toBe(
      normalizeMultilineString(expected)
    );
  });

  it("should convert pasted list of resources", async () => {
    const json = `
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

    const waitfor = "resource stg1";
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

    await configureSettings();

    const textDocument = await vscode.workspace.openTextDocument({
      language: "bicep",
      content: "// My bicep file\n",
    });
    await vscode.window.showTextDocument(textDocument);

    // Make sure Decompile on Paste is on
    await getBicepConfiguration().update(
      "decompileOnPaste",
      true,
      ConfigurationTarget.Global
    );

    vscode.env.clipboard.writeText(json);
    await vscode.commands.executeCommand("editor.action.clipboardPasteAction");

    await until(() => textDocument.getText().includes(waitfor), {
      timeoutMs: 10000,
    });
    const buffer = textDocument.getText();

    expect(normalizeMultilineString(buffer)).toBe(
      normalizeMultilineString(expected)
    );
  });
});
