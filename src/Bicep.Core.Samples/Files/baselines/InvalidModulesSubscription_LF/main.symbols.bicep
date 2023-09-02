targetScope = 'subscription'

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName1. Type: module. Declaration start char: 0, length: 178
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName2. Type: module. Declaration start char: 0, length: 178
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName3. Type: module. Declaration start char: 0, length: 140
  name: 'subscriptionModuleDuplicateName'
  scope: subscription()
}
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName4. Type: module. Declaration start char: 0, length: 140
  name: 'subscriptionModuleDuplicateName'
  scope: subscription()
}
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName5. Type: module. Declaration start char: 0, length: 116
  name: 'subscriptionModuleDuplicateName'
}

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
//@[7:40) Module resourceGroupModuleDuplicateName1. Type: module. Declaration start char: 0, length: 148
  name: 'resourceGroupModuleDuplicateName'
  scope: resourceGroup('RG')
}
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
//@[7:40) Module resourceGroupModuleDuplicateName2. Type: module. Declaration start char: 0, length: 148
  name: 'resourceGroupModuleDuplicateName'
  scope: resourceGroup('RG')
}

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
//@[7:38) Module unsupportedScopeManagementGroup. Type: module. Declaration start char: 0, length: 149
  name: 'unsupportedScopeManagementGroup'
  scope: managementGroup('MG')
}

module singleRgModule 'modules/passthrough.bicep' = {
//@[7:21) Module singleRgModule. Type: module. Declaration start char: 0, length: 143
  name: 'single-rg'
  params: {
    myInput: 'stuff'
  }
  scope: resourceGroup('test')
}

module singleRgModule2 'modules/passthrough.bicep' = {
//@[7:22) Module singleRgModule2. Type: module. Declaration start char: 0, length: 138
  name: 'single-rg2'
  params: {
    myInput: 'stuff'
  }
  scope: singleRgModule
}

