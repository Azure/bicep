{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "13791910200923239863"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myTenantMod",
      "scope": "/",
      "location": "[resourceGroup().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "tenant.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myOtherResourceGroup",
      "subscriptionId": "db90cfef-a146-4f67-b32f-b263518bd216",
      "resourceGroup": "myOtherRg",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "resourcegroup_other.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "mySubscription",
      "subscriptionId": "[subscription().subscriptionId]",
      "location": "[resourceGroup().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "subscription_empty.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "otherSubscription",
      "subscriptionId": "cd780357-07f5-49cc-b945-a3fe15863860",
      "location": "[resourceGroup().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "subscription_empty.bicep"
        }
      }
    }
  ],
  "outputs": {
    "myOutput": {
      "type": "string",
      "value": "[reference(tenantResourceId('Microsoft.Resources/deployments', 'myTenantMod'), '2025-04-01').outputs.myOutput.value]"
    },
    "myOutputResourceGroup": {
      "type": "string",
      "value": "[reference(extensionResourceId(format('/subscriptions/{0}/resourceGroups/{1}', 'db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg'), 'Microsoft.Resources/deployments', 'myOtherResourceGroup'), '2025-04-01').outputs.myOutput.value]"
    }
  }
}