{
  "$schema": "https://schema.management.azure.com/schemas/2018-05-01/subscriptionDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "14256939266001777126"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myResourceGroupMod",
      "resourceGroup": "myRg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "resourcegroup.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myResourceGroupMod2",
      "resourceGroup": "myRg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "resourcegroup.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myResourceGroupMod3",
      "subscriptionId": "subId",
      "resourceGroup": "myRg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "resourcegroup.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myTenantMod",
      "scope": "/",
      "location": "[deployment().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "tenant.bicep"
        }
      }
    }
  ],
  "outputs": {
    "myOutput": {
      "type": "string",
      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2025-04-01').outputs.myOutput.value]"
    },
    "myOutputRgMod": {
      "type": "string",
      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod'), '2025-04-01').outputs.myOutput.value]"
    },
    "myOutputRgMod2": {
      "type": "string",
      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', subscription().subscriptionId, 'myRg'), 'Microsoft.Resources/deployments', 'myResourceGroupMod2'), '2025-04-01').outputs.myOutput.value]"
    }
  }
}