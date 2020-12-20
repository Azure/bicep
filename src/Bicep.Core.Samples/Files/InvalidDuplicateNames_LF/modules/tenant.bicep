targetScope = 'tenant'

module tenantModuleDup1 'empty/tenant.bicep' = {
  name: 'tenantDup'
  scope: tenant()
}

module tenantModuleDup2 'empty/tenant.bicep' = {
  name: 'tenantDup'
  scope: tenant()
}

module tenantModuleDup3 'empty/tenant.bicep' = {
  name: 'tenantDup'
}

module managementGroupModuleDup1 'empty/managementGroup.bicep' = {
  name: 'managementGroupDup'
  scope: managementGroup('MG')
}

module managementGroupModuleDup2 'empty/managementGroup.bicep' = {
  name: 'managementGroupDup'
  scope: managementGroup('MG')
}

module subscriptionModuleDup1 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module subscriptionModuleDup2 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}
