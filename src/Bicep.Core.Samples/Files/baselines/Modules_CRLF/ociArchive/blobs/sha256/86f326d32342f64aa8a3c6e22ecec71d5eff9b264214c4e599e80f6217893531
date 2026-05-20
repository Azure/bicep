{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "4191259681487754679"
    }
  },
  "parameters": {
    "optionalString": {
      "type": "string",
      "defaultValue": "abc"
    },
    "optionalInt": {
      "type": "int",
      "defaultValue": 42
    },
    "optionalObj": {
      "type": "object",
      "defaultValue": {
        "a": "b"
      }
    },
    "optionalArray": {
      "type": "array",
      "defaultValue": [
        1,
        2,
        3
      ]
    }
  },
  "resources": [],
  "outputs": {
    "outputObj": {
      "type": "object",
      "value": {
        "optionalString": "[parameters('optionalString')]",
        "optionalInt": "[parameters('optionalInt')]",
        "optionalObj": "[parameters('optionalObj')]",
        "optionalArray": "[parameters('optionalArray')]"
      }
    }
  }
}