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
