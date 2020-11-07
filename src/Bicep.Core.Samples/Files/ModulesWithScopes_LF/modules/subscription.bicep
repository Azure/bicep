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

output myOutput string = myTenantMod.outputs.myOutput
output myOutputRgMod string = myResourceGroupMod.outputs.myOutput
output myOutputRgMod2 string = myResourceGroupMod2.outputs.myOutput