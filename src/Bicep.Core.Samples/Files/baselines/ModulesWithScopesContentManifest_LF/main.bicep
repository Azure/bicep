targetScope = 'tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup')
}
module myManagementGroupModWithDuplicatedNameButDifferentScope 'modules/managementgroup_empty.bicep' = {
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionMod 'modules/subscription.bicep' = {
  name: 'mySubscriptionMod'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithCondition 'modules/subscription.bicep' = if (length('foo') == 3) {
  name: 'mySubscriptionModWithCondition'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

module mySubscriptionModWithDuplicatedNameButDifferentScope 'modules/subscription_empty.bicep' = {
  name: 'mySubscriptionMod'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}


output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
