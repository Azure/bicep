targetScope = 'managementGroup'

module managementGroupModuleDup1 'empty/managementGroup.bicep' = {
  name: 'managementGroupDup'
  scope: managementGroup()
}

module managementGroupModuleDup2 'empty/managementGroup.bicep' = {
  name: 'managementGroupDup'
  scope: managementGroup()
}

module managementGroupModuleDup3 'empty/managementGroup.bicep' = {
  name: 'managementGroupDup'
}

module subscriptionModuleDup1 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module subscriptionModuleDup2 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}
