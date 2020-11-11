targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[7:27) Module myManagementGroupMod. Type: module. Declaration start char: 0, length: 142
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup')
}

module mySubscriptionMod 'modules/subscription.bicep' = {
//@[7:24) Module mySubscriptionMod. Type: module. Declaration start char: 0, length: 149
  name: 'mySubscriptionMod'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[7:30) Output myManagementGroupOutput. Type: string. Declaration start char: 0, length: 77
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[7:27) Output mySubscriptionOutput. Type: string. Declaration start char: 0, length: 71
