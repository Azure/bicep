{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "8300391961099598421"
    }
  },
  "parameters": {
    "stringParamA": {
      "type": "string",
      "defaultValue": "test"
    },
    "stringParamB": {
      "type": "string"
    },
    "objParam": {
      "type": "object"
    },
    "arrayParam": {
      "type": "array"
    }
  },
  "resources": [
    {
      "type": "Mock.Rp/mockResource",
      "apiVersion": "2020-01-01",
      "name": "basicblobs",
      "location": "[parameters('stringParamA')]"
    },
    {
      "type": "Mock.Rp/mockResource",
      "apiVersion": "2020-01-01",
      "name": "myZone",
      "location": "[parameters('stringParamB')]"
    }
  ],
  "outputs": {
    "stringOutputA": {
      "type": "string",
      "value": "[parameters('stringParamA')]"
    },
    "stringOutputB": {
      "type": "string",
      "value": "[parameters('stringParamB')]"
    },
    "objOutput": {
      "type": "object",
      "value": "[reference(resourceId('Mock.Rp/mockResource', 'basicblobs'), '2020-01-01')]"
    },
    "arrayOutput": {
      "type": "array",
      "value": [
        "[resourceId('Mock.Rp/mockResource', 'basicblobs')]",
        "[resourceId('Mock.Rp/mockResource', 'myZone')]"
      ]
    }
  }
}