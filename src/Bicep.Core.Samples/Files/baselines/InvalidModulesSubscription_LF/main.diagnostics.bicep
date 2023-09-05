targetScope = 'subscription'

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription()
}
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription()
}
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
}

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
//@[8:42) [BCP122 (Error)] Modules: "resourceGroupModuleDuplicateName1", "resourceGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'resourceGroupModuleDuplicateName'|
  scope: resourceGroup('RG')
}
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
//@[8:42) [BCP122 (Error)] Modules: "resourceGroupModuleDuplicateName1", "resourceGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'resourceGroupModuleDuplicateName'|
  scope: resourceGroup('RG')
}

module unsupportedScopeManagementGroup 'modules/managementGroup.bicep' = {
  name: 'unsupportedScopeManagementGroup'
  scope: managementGroup('MG')
//@[9:30) [BCP115 (Error)] Unsupported scope for module deployment in a "subscription" target scope. Omit this property to inherit the current scope, or specify a valid scope. Permissible scopes include current subscription: subscription(), named subscription: subscription(<subId>), named resource group in same subscription: resourceGroup(<name>), named resource group in different subscription: resourceGroup(<subId>, <name>), or tenant: tenant(). (CodeDescription: none) |managementGroup('MG')|
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
//@[9:23) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "resourceGroup". (CodeDescription: none) |singleRgModule|
}

