// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type {
  DeploymentExtended,
  DeploymentOperation,
  DeploymentValidateResult,
  WhatIfOperationResult,
} from "@azure/arm-resources";
import type { DeploymentScope } from "../models";

export const fileUri = "file:///my/deployment.bicep";

export const templateJson = `{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "fooParam": {
      "type": "string"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2023-01-01",
      "name": "[parameters('fooParam')]",
      "location": "West US",
      "sku": {
        "name": "Standard_ZRS"
      },
      "kind": "StorageV2"
    }
  ]
}`;

export const parametersJson = `{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "fooParam": {
      "value": "ASDASD"
    }
  }
}`;

export const scope: DeploymentScope & { scopeType: "resourceGroup" } = {
  scopeType: "resourceGroup",
  tenantId: "9eeeb42f-40a5-4229-849f-a71fae26c89f",
  subscriptionId: "a790367f-228f-43fd-a660-ed9abb7258ef",
  resourceGroup: "mockRg",
  armUrl: "https://management.azure.com",
  portalUrl: "https://portal.azure.com",
};

export function getDeployResponse(): DeploymentExtended {
  return {
    properties: {
      outputs: {
        foo: {
          value: "bar",
        },
      },
    },
  };
}

export function getValidateResponse(): DeploymentValidateResult {
  return {
    error: {
      code: "InvalidTemplate",
      message: "The template is invalid",
    },
  };
}

export function getWhatIfResponse(): WhatIfOperationResult {
  return {
    changes: [
      {
        changeType: "Create",
        resourceId: `/subscriptions/${scope.subscriptionId}/resourceGroups/${scope.resourceGroup}/providers/Microsoft.Storage/storageAccounts/fooAccount`,
        delta: [
          {
            propertyChangeType: "Create",
            path: "properties",
            before: {
              accountType: "Standard_LRS",
              enableHttpsTrafficOnly: false,
            },
            after: {
              accountType: "Standard_ZRS",
              enableHttpsTrafficOnly: true,
            },
          },
        ],
      },
    ],
  };
}

export async function* getDeploymentOperations(): AsyncIterable<DeploymentOperation> {
  yield {
    properties: {
      provisioningOperation: "Create",
      provisioningState: "Succeeded",
      targetResource: {
        resourceType: "Microsoft.Storage/storageAccounts",
        resourceName: "fooAccount",
        id: `/subscriptions/${scope.subscriptionId}/resourceGroups/${scope.resourceGroup}/providers/Microsoft.Storage/storageAccounts/fooAccount`,
      },
    },
  };
}
