{
  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/managementGroupDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "1608610798337509733"
    }
  },
  "resources": [
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
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myManagementGroupMod",
      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
      "location": "[deployment().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "managementgroup_empty.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "mySubscriptionMod",
      "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
      "location": "[deployment().location]",
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
    }
  }
}