{
  "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "12537675657486909114"
    }
  },
  "parameters": {
    "prefix": {
      "type": "string",
      "defaultValue": "majastrz"
    }
  },
  "variables": {
    "groups": [
      "bicep1",
      "bicep2",
      "bicep3",
      "bicep4"
    ],
    "scripts": "[take(variables('groups'), 2)]"
  },
  "resources": [
    {
      "copy": {
        "name": "resourceGroups",
        "count": "[length(variables('groups'))]"
      },
      "type": "Microsoft.Resources/resourceGroups",
      "apiVersion": "2020-06-01",
      "name": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
      "location": "westus"
    },
    {
      "copy": {
        "name": "scopedToSymbolicName",
        "count": "[length(variables('scripts'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
      "resourceGroup": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "scriptName": {
            "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
          }
        },
        "templateLink": {
          "relativePath": "hello.bicep"
        }
      },
      "dependsOn": [
        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()]))]"
      ]
    },
    {
      "copy": {
        "name": "scopedToResourceGroupFunction",
        "count": "[length(variables('scripts'))]"
      },
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
      "resourceGroup": "[concat(variables('scripts')[copyIndex()], '-extra')]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "scriptName": {
            "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
          }
        },
        "templateLink": {
          "relativePath": "hello.bicep"
        }
      }
    }
  ]
}