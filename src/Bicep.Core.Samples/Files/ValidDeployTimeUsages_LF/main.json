{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "2540193526821897161"
    }
  },
  "parameters": {
    "cond": {
      "type": "bool",
      "defaultValue": false
    }
  },
  "variables": {
    "copy": [
      {
        "name": "varForBodyOkDeployTimeUsageExpression",
        "count": "[length(range(0, 2))]",
        "input": "[resourceId('Microsoft.Storage/storageAccounts', 'foo')]"
      },
      {
        "name": "varForBodyOkDeployTimeUsageInterpolatedKey",
        "count": "[length(range(0, 2))]",
        "input": {
          "[format('{0}', resourceId('Microsoft.Storage/storageAccounts', 'foo'))]": "id"
        }
      }
    ],
    "zeroIndex": 0,
    "otherIndex": "[add(variables('zeroIndex'), 2)]",
    "idAccessor": "id",
    "dStr": "d",
    "idAccessor2": "[variables('idAccessor')]",
    "idAccessorInterpolated": "[format('{0}', variables('idAccessor'))]",
    "idAccessorMixed": "[format('i{0}', variables('dStr'))]",
    "strArray": [
      "id",
      "properties"
    ]
  },
  "resources": [
    {
      "type": "Microsoft.Storage/storageAccounts/fileServices",
      "apiVersion": "2022-09-01",
      "name": "[format('{0}/{1}', 'foo', 'default')]",
      "dependsOn": [
        "[resourceId('Microsoft.Storage/storageAccounts', 'foo')]"
      ]
    },
    {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2022-09-01",
      "name": "foo",
      "location": "westus",
      "sku": {
        "name": "Standard_LRS"
      },
      "kind": "StorageV2"
    },
    {
      "copy": {
        "name": "foos",
        "count": "[length(range(0, 2))]"
      },
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2022-09-01",
      "name": "[format('foo-{0}', range(0, 2)[copyIndex()])]",
      "location": "westus",
      "sku": {
        "name": "Standard_LRS"
      },
      "kind": "StorageV2"
    }
  ]
}