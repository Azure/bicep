{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "15522334618541518671"
    }
  },
  "parameters": {
    "secureStringParam1": {
      "type": "securestring"
    },
    "secureStringParam2": {
      "type": "securestring",
      "defaultValue": ""
    }
  },
  "resources": [],
  "outputs": {
    "exposedSecureString": {
      "type": "string",
      "value": "[parameters('secureStringParam1')]"
    }
  }
}