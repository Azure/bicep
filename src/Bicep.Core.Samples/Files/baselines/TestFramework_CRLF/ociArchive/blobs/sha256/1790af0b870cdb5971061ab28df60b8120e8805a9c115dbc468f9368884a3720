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
      "templateHash": "11988534461849735373"
    }
  },
  "parameters": {
    "location": {
      "type": "string"
    }
  },
  "resources": {
    "storageAccount": {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2022-09-01",
      "name": "toylaunchstorage",
      "location": "[parameters('location')]",
      "sku": {
        "name": "Standard_LRS"
      },
      "kind": "StorageV2",
      "properties": {
        "accessTier": "Hot"
      }
    }
  },
  "asserts": {
    "storageAccountNameIsCorrect": "[equals('toylaunchstorage', 'toylaunchstorage2')]",
    "storageAccountLocationIsCorrect": "[equals(reference('storageAccount', '2022-09-01', 'full').location, 'westus3')]"
  }
}