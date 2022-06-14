targetScope = 'tenant'
//@[246:1058]                             "myOutput": {

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[11:1153]       "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[14:1004]       "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[64:148]               "type": "Microsoft.Resources/deployments",
  name: 'myManagementGroupMod'
//@[67:1062]               "name": "myManagementGroupMod",
  scope: managementGroup('myManagementGroup2')
//@[226:1035]                       "name": "myOtherResourceGroup",
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[89:661]               "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[92:152]               "name": "mySubscriptionMod",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[254:1126]                       "type": "Microsoft.Resources/deployments",
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[116:1175]             "myOutput": {
  name: 'mySubscriptionModWithCondition'
//@[666:666]       "name": "mySubscriptionModWithCondition",
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[279:1112]                       "type": "Microsoft.Resources/deployments",
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[1176:1200]       "type": "Microsoft.Resources/deployments",
  name: 'mySubscriptionMod'
//@[1179:1179]       "name": "mySubscriptionMod",
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[306:1118]                     "myOutput": {
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[1203:1206]     "myManagementGroupOutput": {
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[1207:1210]     "mySubscriptionOutput": {

