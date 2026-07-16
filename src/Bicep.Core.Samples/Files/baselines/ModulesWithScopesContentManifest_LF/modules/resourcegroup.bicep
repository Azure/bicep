targetScope = 'resourceGroup'

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}

module myOtherResourceGroup 'resourcegroup_other.bicep' = {
  name: 'myOtherResourceGroup'
  scope: resourceGroup('db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg')
}

module mySubscription 'subscription_empty.bicep' = {
  name: 'mySubscription'
  scope: subscription()
}

module otherSubscription 'subscription_empty.bicep' = {
  name: 'otherSubscription'
  scope: subscription('cd780357-07f5-49cc-b945-a3fe15863860')
}

output myOutput string = myTenantMod.outputs.myOutput
output myOutputResourceGroup string = myOtherResourceGroup.outputs.myOutput