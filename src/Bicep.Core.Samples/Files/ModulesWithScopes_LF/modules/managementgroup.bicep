targetScope = 'managementGroup'

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}