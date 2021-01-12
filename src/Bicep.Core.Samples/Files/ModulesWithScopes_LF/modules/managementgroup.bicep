targetScope = 'managementGroup'

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}
module myManagementGroupModWithDuplicatedNameButInSeparateModule 'managementgroup_empty.bicep' = {
  name: 'myManagementGroupMod'
  scope: managementGroup('myManagementGroup2')
}
module mySubscriptionModWithDuplicatedNameButInSeparateModule 'subscription_empty.bicep' = {
  name: 'mySubscriptionMod'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

output myOutput string = myTenantMod.outputs.myOutput
