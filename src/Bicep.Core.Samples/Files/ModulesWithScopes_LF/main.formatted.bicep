targetScope='tenant'

module myManagementGroupMod 'modules/managementgroup.bicep' = {
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup')
}

module mySubscriptionMod 'modules/subscription.bicep' = {
  name: 'mySubscriptionMod'
  scope: subscription('ee44cd78-68c6-43d9-874e-e684ec8d1191')
}

output myManagementGroupOutput string = myManagementGroupMod.outputs.myOutput
output mySubscriptionOutput string = mySubscriptionMod.outputs.myOutput
