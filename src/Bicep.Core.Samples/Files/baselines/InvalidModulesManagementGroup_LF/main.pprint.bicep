targetScope = 'managementGroup'

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup()
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup()
}

module managementGroupModuleDuplicateName3 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionDuplicateName'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionDuplicateName'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
  name: 'tenantDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
  name: 'tenantDuplicateName'
  scope: tenant()
}
