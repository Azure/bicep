// REP 0015: the classic "hard" scope expression. A conditional (ternary) scope that today fails with
// BCP420 ("scope could not be resolved at compile time") now compiles: both branches are ResourceScope
// members sharing the 'resourceGroup' discriminator, so the whole expression is emitted verbatim into
// "@scope" and the deployment engine resolves it at deploy time.
param otherResourceGroup string = ''
//@    "otherResourceGroup": {
//@      "type": "string",
//@      "defaultValue": ""
//@    }

module mod 'modules/mod.bicep' = {
//@    "mod": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "@scope": "[if(not(empty(parameters('otherResourceGroup'))), resourceGroup(parameters('otherResourceGroup')), resourceGroup())]",
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
//@              "templateHash": "6106840436592282640"
//@            }
//@          },
//@          "resources": {},
//@          "outputs": {
//@            "done": {
//@              "type": "bool",
//@              "value": true
//@            }
//@          }
//@        }
//@      }
//@    }
  name: 'mod'
//@      "name": "mod",
  scope: !empty(otherResourceGroup) ? resourceGroup(otherResourceGroup) : resourceGroup()
}

