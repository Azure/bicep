// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { IActionContext } from "@microsoft/vscode-azext-utils";
import vscode from "vscode";
import { Command } from "../types";

const paramsCode =
  "param location string = resourceGroup().location\n" +
  "param appPlanName string = 'myAppPlanName'\n" +
  "\n";

const resourcesCode = `
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
`;

export class WalkthroughCopyToClipboardCommand implements Command {
  public readonly id = "bicep.gettingStarted.copyToClipboard";

  public async execute(
    context: IActionContext,
    args: { step: "params" | "resources" }
  ): Promise<void> {
    const step = args.step;
    context.telemetry.properties.step = step;

    console.log(
      encodeURIComponent(JSON.stringify(["ms-vscode.azure-account"]))
    );

    const code = step === "params" ? paramsCode : resourcesCode;
    vscode.env.clipboard.writeText(code);
  }
}
