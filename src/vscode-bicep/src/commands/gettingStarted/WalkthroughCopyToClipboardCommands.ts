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
    vscode.env.clipboard.writeText(
      `resource storageaccount 'Microsoft.Storage/storageAccounts@2021-02-01' = {
  name: 'name'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Premium_LRS'
  }
}
`
    );
  }
}
