targetScope = 'managementGroup'

module myTenantMod 'tenant.bicep' = {
  name: 'myTenantMod'
  scope: tenant()
}

output myOutput string = myTenantMod.outputs.myOutput