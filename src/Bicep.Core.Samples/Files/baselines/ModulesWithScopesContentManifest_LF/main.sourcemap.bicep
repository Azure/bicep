targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@    "myManagementGroupMod": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup')]",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "contentId": "sha256:b0e48580ec85bc7699c01af9e91baaa9334c074fa4b1241d39cdd0b25e7b69b7"
//@        }
//@      }
//@    },
  name: 'myManagementGroupMod'
//@      "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@    "myManagementGroupModWithDuplicatedNameButDifferentScope": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "scope": "[format('Microsoft.Management/managementGroups/{0}', 'myManagementGroup2')]",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "contentId": "sha256:bd47cb5084e5d04753b7182536f3cd22b11ad5a276e72dd2f732399aef62ae9e"
//@        }
//@      }
//@    },
  name: 'myManagementGroupMod'
//@      "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@    "mySubscriptionMod": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "contentId": "sha256:2f751175cb9be0ad11a72de9fca59bf11ec3565b3fb1f2892483c0026c2352aa"
//@        }
//@      }
//@    },
  name: 'mySubscriptionMod'
//@      "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@    "mySubscriptionModWithCondition": {
//@      "condition": "[equals(length('foo'), 3)]",
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "contentId": "sha256:2f751175cb9be0ad11a72de9fca59bf11ec3565b3fb1f2892483c0026c2352aa"
//@        }
//@      }
//@    },
  name: 'mySubscriptionModWithCondition'
//@      "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@      "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@    "mySubscriptionModWithDuplicatedNameButDifferentScope": {
//@      "type": "Microsoft.Resources/deployments",
//@      "apiVersion": "2025-04-01",
//@      "location": "[deployment().location]",
//@      "properties": {
//@        "expressionEvaluationOptions": {
//@          "scope": "inner"
//@        },
//@        "mode": "Incremental",
//@        "templateLink": {
//@          "contentId": "sha256:d365b89e1f679dd5ca5bfec42c89146fd1f66e650b1c54fd7f2fda13dce48bcf"
//@        }
//@      }
//@    }
  name: 'mySubscriptionMod'
//@      "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@      "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@    "myManagementGroupOutput": {
//@      "type": "string",
//@      "value": "[reference('myManagementGroupMod').outputs.myOutput.value]"
//@    },
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@    "mySubscriptionOutput": {
//@      "type": "string",
//@      "value": "[reference('mySubscriptionMod').outputs.myOutput.value]"
//@    }

