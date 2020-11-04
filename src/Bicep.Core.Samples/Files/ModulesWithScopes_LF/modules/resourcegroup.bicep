targetScope = 'resourceGroup'

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}

module myOtherResourceGroup 'resourcegroup_other.bicep' = {
  name: 'myOtherResourceGroup'
  scope: resourceGroup('db90cfef-a146-4f67-b32f-b263518bd216', 'myOtherRg')
}