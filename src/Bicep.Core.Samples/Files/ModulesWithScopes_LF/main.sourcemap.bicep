targetScope = 'tenant'
//@[246:1058]                             "myOutput": {\r

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[11:1153]       "type": "Microsoft.Resources/deployments",\r
  name: 'myManagementGroupMod'
//@[14:1004]       "name": "myManagementGroupMod",\r
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[64:148]               "type": "Microsoft.Resources/deployments",\r
  name: 'myManagementGroupMod'
//@[67:1062]               "name": "myManagementGroupMod",\r
  scope: managementGroup('myManagementGroup2')
//@[226:1035]                       "name": "myOtherResourceGroup",\r
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[89:661]               "type": "Microsoft.Resources/deployments",\r
  name: 'mySubscriptionMod'
//@[92:152]               "name": "mySubscriptionMod",\r
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[254:1126]                       "type": "Microsoft.Resources/deployments",\r
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[116:1175]             "myOutput": {\r
  name: 'mySubscriptionModWithCondition'
//@[666:666]       "name": "mySubscriptionModWithCondition",\r
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
//@[279:1112]                       "type": "Microsoft.Resources/deployments",\r
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[1176:1200]       "type": "Microsoft.Resources/deployments",\r
  name: 'mySubscriptionMod'
//@[1179:1179]       "name": "mySubscriptionMod",\r
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
//@[306:1118]                     "myOutput": {\r
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[1203:1206]     "myManagementGroupOutput": {\r
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[1207:1210]     "mySubscriptionOutput": {\r

