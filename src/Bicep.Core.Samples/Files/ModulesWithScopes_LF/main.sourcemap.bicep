targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[12:124]       "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[15:15]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[125:149]       "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[128:128]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[150:662]       "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[153:153]       "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[663:1176]       "condition": "[equals(length('foo'), 3)]",
  name: 'mySubscriptionModWithCondition'
//@[667:667]       "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[1177:1201]       "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[1180:1180]       "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[1204:1207]     "myManagementGroupOutput": {
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[1208:1211]     "mySubscriptionOutput": {

