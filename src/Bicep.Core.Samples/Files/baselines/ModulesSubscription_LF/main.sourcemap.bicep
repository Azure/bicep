targetScope = 'subscription'

param prefix string = 'majastrz'
//@    "prefix": {
//@      "type": "string",
//@      "defaultValue": "majastrz"
//@    }
var groups = [
//@    "groups": [
//@    ],
  'bicep1'
//@      "bicep1",
  'bicep2'
//@      "bicep2",
  'bicep3'
//@      "bicep3",
  'bicep4'
//@      "bicep4"
]

var scripts = take(groups, 2)
//@    "scripts": "[take(variables('groups'), 2)]"

resource resourceGroups 'Microsoft.Resources/resourceGroups@2020-06-01' = [for name in groups: {
//@    {
//@      "copy": {
//@        "name": "resourceGroups",
//@        "count": "[length(variables('groups'))]"
//@      },
//@      "type": "Microsoft.Resources/resourceGroups",
//@      "apiVersion": "2020-06-01",
//@      "name": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
//@    },
  name: '${prefix}-${name}'
  location: 'westus'
//@      "location": "westus"
}]

module scopedToSymbolicName 'hello.bicep' = [for (name, i) in scripts: {
//@    {
//@      "copy": {
//@        "name": "scopedToSymbolicName",
//@        "count": "[length(variables('scripts'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "[format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()])]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "9518912405470532169"
//@            }
//@          },
//@          "parameters": {
//@            "scriptName": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Resources/deploymentScripts",
//@              "apiVersion": "2020-10-01",
//@              "name": "[parameters('scriptName')]",
//@              "kind": "AzurePowerShell",
//@              "location": "[resourceGroup().location]",
//@              "properties": {
//@                "azPowerShellVersion": "3.0",
//@                "retentionInterval": "PT6H",
//@                "scriptContent": "      Write-Output 'Hello World!'\n"
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('scriptName')]"
//@            }
//@          }
//@        }
//@      },
//@      "dependsOn": [
//@        "[subscriptionResourceId('Microsoft.Resources/resourceGroups', format('{0}-{1}', parameters('prefix'), variables('groups')[copyIndex()]))]"
//@      ]
//@    },
  name: '${prefix}-dep-${i}'
//@      "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
//@        "parameters": {
//@        },
    scriptName: 'test-${name}-${i}'
//@          "scriptName": {
//@            "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
//@          }
  }
  scope: resourceGroups[i]
}]

module scopedToResourceGroupFunction 'hello.bicep' = [for (name, i) in scripts: {
//@    {
//@      "copy": {
//@        "name": "scopedToResourceGroupFunction",
//@        "count": "[length(variables('scripts'))]"
//@      },
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "resourceGroup": "[concat(variables('scripts')[copyIndex()], '-extra')]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "9518912405470532169"
//@            }
//@          },
//@          "parameters": {
//@            "scriptName": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": [
//@            {
//@              "type": "Microsoft.Resources/deploymentScripts",
//@              "apiVersion": "2020-10-01",
//@              "name": "[parameters('scriptName')]",
//@              "kind": "AzurePowerShell",
//@              "location": "[resourceGroup().location]",
//@              "properties": {
//@                "azPowerShellVersion": "3.0",
//@                "retentionInterval": "PT6H",
//@                "scriptContent": "      Write-Output 'Hello World!'\n"
//@              }
//@            }
//@          ],
//@          "outputs": {
//@            "myOutput": {
//@              "type": "string",
//@              "value": "[parameters('scriptName')]"
//@            }
//@          }
//@        }
//@      }
//@    }
  name: '${prefix}-dep-${i}'
//@      "name": "[format('{0}-dep-{1}', parameters('prefix'), copyIndex())]",
  params: {
//@        "parameters": {
//@        },
    scriptName: 'test-${name}-${i}'
//@          "scriptName": {
//@            "value": "[format('test-{0}-{1}', variables('scripts')[copyIndex()], copyIndex())]"
//@          }
  }
  scope: resourceGroup(concat(name, '-extra'))
}]


