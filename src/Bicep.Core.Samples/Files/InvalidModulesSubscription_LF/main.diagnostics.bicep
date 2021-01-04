targetScope = 'subscription'

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription('ced92236-c4d9-46ab-a299-a59c387fd1ee')
}

module subscriptionModuleDuplicateName3 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription()
}
module subscriptionModuleDuplicateName4 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription()
}
module subscriptionModuleDuplicateName5 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName3", "subscriptionModuleDuplicateName4", "subscriptionModuleDuplicateName5" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
}

module resourceGroupModuleDuplicateName1 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
//@[8:42) [BCP121 (Error)] Modules: "resourceGroupModuleDuplicateName1", "resourceGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'resourceGroupModuleDuplicateName'|
  scope: resourceGroup('RG')
}
module resourceGroupModuleDuplicateName2 'modules/resourceGroup.bicep' = {
  name: 'resourceGroupModuleDuplicateName'
//@[8:42) [BCP121 (Error)] Modules: "resourceGroupModuleDuplicateName1", "resourceGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'resourceGroupModuleDuplicateName'|
  scope: resourceGroup('RG')
}

