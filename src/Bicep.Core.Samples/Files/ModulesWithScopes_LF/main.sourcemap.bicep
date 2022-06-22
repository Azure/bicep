targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[11:123]       "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[14:14]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[124:148]       "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[127:127]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[149:661]       "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[152:152]       "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[153:153]       "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[662:1175]       "condition": "[equals(length('foo'), 3)]",
  name: 'mySubscriptionModWithCondition'
//@[666:666]       "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[667:667]       "subscriptionId": "ee44cd78-68c6-43d9-874e-e684ec8d1191",
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[1176:1200]       "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[1179:1179]       "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[1180:1180]       "subscriptionId": "1ad827ac-2669-4c2f-9970-282b93c3c550",
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[1203:1206]     "myManagementGroupOutput": {
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[1207:1210]     "mySubscriptionOutput": {

