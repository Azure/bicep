targetScope = 'subscription'

param location string = 'eastus'
//@    "location": {
//@      "type": "string",
//@      "defaultValue": "eastus"
//@    }

// REP 0015: with the 'formalizedScope' experimental feature enabled, this module's cross-scope
// targeting is emitted as a single duck-typed "@scope" object instead of the legacy
// "subscriptionId" / "resourceGroup" properties.
module storageMod 'modules/mod.bicep' = {
//@    "storageMod": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "@scope": "[resourceGroup('my-rg')]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "template": {
//@          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
//@          "languageVersion": "2.1-experimental",
//@          "contentVersion": "1.0.0.0",
//@          "metadata": {
//@            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
//@            "_EXPERIMENTAL_FEATURES_ENABLED": [
//@              "Enable formalized @scope handling"
//@            ],
//@            "_generator": {
//@              "name": "bicep",
//@              "version": "dev",
//@              "templateHash": "11927649277441762896"
//@            }
//@          },
//@          "parameters": {
//@            "location": {
//@              "type": "string"
//@            }
//@          },
//@          "resources": {},
//@          "outputs": {
//@            "loc": {
//@              "type": "string",
//@              "value": "[parameters('location')]"
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'storageMod'
//@      "name": "storageMod",
  scope: resourceGroup('my-rg')
  params: {
//@        "parameters": {
//@        },
    location: location
//@          "location": {
//@            "value": "[parameters('location')]"
//@          }
  }
}

output loc string = storageMod.outputs.loc
//@    "loc": {
//@      "type": "string",
//@      "value": "[reference('storageMod').outputs.loc.value]"
//@    }

