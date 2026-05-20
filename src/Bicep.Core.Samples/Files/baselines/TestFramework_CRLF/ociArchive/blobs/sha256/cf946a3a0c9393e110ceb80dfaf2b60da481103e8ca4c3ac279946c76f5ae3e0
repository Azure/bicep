{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.1-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
    "_EXPERIMENTAL_FEATURES_ENABLED": [
      "Asserts"
    ],
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "14301882665059122893"
    }
  },
  "parameters": {
    "env": {
      "type": "string"
    },
    "suffix": {
      "type": "int"
    }
  },
  "variables": {
    "name": "[format('{0}-solution-app', parameters('env'))]",
    "nameWithSuffix": "[format('{0}-{1}', variables('name'), parameters('suffix'))]",
    "location": "[if(or(equals(parameters('env'), 'prod'), equals(parameters('env'), 'main')), 'eastus', 'westus')]"
  },
  "resources": {},
  "asserts": {
    "nameIsCorrect": "[or(equals(variables('name'), 'dev-solution-app'), equals(variables('name'), 'prod-solution-app'))]",
    "nameHasValidEnv": "[or(or(contains(variables('name'), 'prod'), contains(variables('name'), 'dev')), contains(variables('name'), 'main'))]",
    "nameContainsSuffix": "[or(or(contains(variables('nameWithSuffix'), '1'), contains(variables('nameWithSuffix'), '2')), contains(variables('nameWithSuffix'), '3'))]"
  }
}