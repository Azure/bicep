// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import vscode from "vscode";
import { Command } from "../types";

export class WalkthroughCopyToClipboardCommandParams implements Command {
  public readonly id = "bicep.gettingStarted.copyToClipboardParams";

  public async execute(): Promise<void> {
    vscode.env.clipboard.writeText(
      "param location string = resourceGroup().location\n" +
        "param appPlanName string\n" +
        "\n"
    );
  }
}

export class WalkthroughCopyToClipboardCommandResources implements Command {
  public readonly id = "bicep.gettingStarted.copyToClipboardResources";

  //asdfg image shows storage account
  public async execute(): Promise<void> {
    vscode.env.clipboard.writeText(`
resource appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appPlanName
  location: location
  sku: {
    name: 'F1'
    capacity: 1
  }
}

resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: '\${appServicePlan.name}storage'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}      
`);
  }
}
