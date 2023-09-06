targetScope = 'subscription'

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription()
}
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
  scope: subscription()
}
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
}

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
  scope: resourceGroup('RG')
}
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
  scope: resourceGroup('RG')
}

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
  name: 'unsupportedScopeManagementGroup'
  scope: managementGroup('MG')
}

module singleRgModule 'modules/passthrough.bicep' = {
  name: 'single-rg'
  params: {
    myInput: 'stuff'
  }
  scope: resourceGroup('test')
}

module singleRgModule2 'modules/passthrough.bicep' = {
  name: 'single-rg2'
  params: {
    myInput: 'stuff'
  }
  scope: singleRgModule
}
