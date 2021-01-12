targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
//@[7:27) Module myManagementGroupMod. Type: module. Declaration start char: 0, length: 142
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
//@[7:62) Module myManagementGroupModWithDuplicatedNameButDifferentScope. Type: module. Declaration start char: 0, length: 184
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
//@[7:24) Module mySubscriptionMod. Type: module. Declaration start char: 0, length: 149
  name: 'mySubscriptionMod'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
//@[7:37) Module mySubscriptionModWithCondition. Type: module. Declaration start char: 0, length: 199
  name: 'mySubscriptionModWithCondition'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
//@[7:59) Module mySubscriptionModWithDuplicatedNameButDifferentScope. Type: module. Declaration start char: 0, length: 190
  name: 'mySubscriptionMod'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
//@[7:30) Output myManagementGroupOutput. Type: string. Declaration start char: 0, length: 77
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
//@[7:27) Output mySubscriptionOutput. Type: string. Declaration start char: 0, length: 71

