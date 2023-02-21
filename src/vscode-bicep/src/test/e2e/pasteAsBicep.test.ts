// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode, { ConfigurationTarget } from "vscode";
import { executeCloseAllEditors } from "./commands";
import { getBicepConfiguration } from "../../language/getBicepConfiguration";
import { until } from "../utils/time";
import { normalizeMultilineString } from "../utils/normalizeMultilineString";
import { SuppressedWarningsManager } from "../../commands/SuppressedWarningsManager";
import { bicepConfigurationKeys } from "../../language/constants";

describe("pasteAsBicep", (): void => {
  afterEach(async () => {
    await executeCloseAllEditors();
  });

  async function configureSettings(): Promise<void> {
    // Make sure Decompile on Paste is on
    await getBicepConfiguration().update(
      bicepConfigurationKeys.decompileOnPaste,
      true,
      ConfigurationTarget.Global
    );

    // Make sure experimental enable paste on bicep is on
    await getBicepConfiguration().update(
      bicepConfigurationKeys.experimentalEnablePasteOnBicep,
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

    await vscode.env.clipboard.writeText(json);
    await vscode.commands.executeCommand("editor.action.clipboardPasteAction");

    try {
      await until(() => textDocument.getText().includes(waitfor), {
        timeoutMs: 10000,
      });
    } catch (err) {
      throw "Timeout.  Editor text: " + textDocument.getText();
    }
    const buffer = textDocument.getText();

    expect(normalizeMultilineString(buffer)).toBe(
      normalizeMultilineString(expected)
    );
  });
});
