targetScope = 'subscription'

module subscriptionModuleDup1 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDup2 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDup3 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription()
}
module subscriptionModuleDup4 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
  scope: subscription()
}
module subscriptionModuleDup5 'empty/subscription.bicep' = {
  name: 'subscriptionDup'
}

module resourceGroupModuleDup1 'empty/resourceGroup.bicep' = {
  name: 'resourceGroupDup'
  scope: resourceGroup('RG')
}
module resourceGroupModuleDup2 'empty/resourceGroup.bicep' = {
  name: 'resourceGroupDup'
  scope: resourceGroup('RG')
}

module resourceGroupModule 'resourceGroup.bicep' = {
  name: 'resourceGroup'
  scope: resourceGroup('RG')
}
