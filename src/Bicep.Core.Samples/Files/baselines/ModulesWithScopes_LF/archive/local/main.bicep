{
  "$schema": "https://schema.management.azure.com/schemas/2019-08-01/tenantDeploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "5055434032715687079"
    }
  },
  "resources": [
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "myManagementGroupMod",
      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup')]",
      "location": "[deployment().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "modules/managementgroup.bicep"
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
          "relativePath": "modules/managementgroup_empty.bicep"
        }
      }
    },
    {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "mySubscriptionMod",
      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
      "location": "[deployment().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "modules/subscription.bicep"
        }
      }
    },
    {
      "condition": "[equals(length('foo'), 3)]",
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2025-04-01",
      "name": "mySubscriptionModWithCondition",
      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
      "location": "[deployment().location]",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "templateLink": {
          "relativePath": "modules/subscription.bicep"
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
          "relativePath": "modules/subscription_empty.bicep"
        }
      }
    }
  ],
  "outputs": {
    "myManagementGroupOutput": {
      "type": "string",
      "value": "[reference(extensionResourceId(tenantResourceId('Microsoft.Management/managementGroups', 'myManagementGroup'), 'Microsoft.Resources/deployments', 'myManagementGroupMod'), '2025-04-01').outputs.myOutput.value]"
    },
    "mySubscriptionOutput": {
      "type": "string",
      "value": "[reference(subscriptionResourceId('ee44cd78-68c6-43d9-874e-e684ec8d1191', 'Microsoft.Resources/deployments', 'mySubscriptionMod'), '2025-04-01').outputs.myOutput.value]"
    }
  }
}