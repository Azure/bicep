targetScope = 'subscription'

module myResourceGroupMod 'resourcegroup.bicep' = {
  name: 'myResourceGroupMod'
  scope: resourceGroup('myRg')
}

module myResourceGroupMod2 'resourcegroup.bicep' = {
  name: 'myResourceGroupMod2'
  scope: resourceGroup(subscription().id, 'myRg')
}

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}