{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "14375999048727010492"
    }
  },
  "parameters": {
    "myInput": {
      "type": "string"
    }
  },
  "resources": [],
  "outputs": {
    "myOutput": {
      "type": "string",
      "value": "[parameters('myInput')]"
    }
  }
}