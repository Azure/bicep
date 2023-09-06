targetScope = 'tenant'

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
}

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup('MG')
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup('MG')
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroup(mg)
}]

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroupModules
}]

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroupModules[i]
}]

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
  name: 'test'
  scope: managementGroupModuleDuplicateName1
}
