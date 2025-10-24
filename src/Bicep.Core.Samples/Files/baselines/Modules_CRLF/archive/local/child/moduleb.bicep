{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "13693869390953445824"
    }
  },
  "parameters": {
    "location": {
      "type": "string"
    }
  },
  "resources": [
    {
      "type": "Mock.Rp/mockResource",
      "apiVersion": "2020-01-01",
      "name": "mockResource",
      "location": "[parameters('location')]"
    }
  ],
  "outputs": {
    "myResourceId": {
      "type": "string",
      "value": "[resourceId('Mock.Rp/mockResource', 'mockResource')]"
    }
  }
}