targetScope = 'tenant'

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP122 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'tenantModuleDuplicateName'|
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP122 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'tenantModuleDuplicateName'|
  scope: tenant()
}

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP122 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'tenantModuleDuplicateName'|
}

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP122 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'managementGroupModuleDuplicateName'|
  scope: managementGroup('MG')
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP122 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'managementGroupModuleDuplicateName'|
  scope: managementGroup('MG')
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP122 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. (CodeDescription: none) |'subscriptionModuleDuplicateName'|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroup(mg)
}]

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroupModules
//@[9:31) [BCP144 (Error)] Directly referencing a resource or module collection is not currently supported. Apply an array indexer to the expression. (CodeDescription: none) |managementGroupModules|
//@[9:31) [BCP036 (Error)] The property "scope" expected a value of type "managementGroup" but the provided value is of type "module[]". (CodeDescription: none) |managementGroupModules|
}]

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
  name: 'dep-${mg}'
  scope: managementGroupModules[i]
//@[9:34) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "managementGroup". (CodeDescription: none) |managementGroupModules[i]|
}]

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
  name: 'test'
  scope: managementGroupModuleDuplicateName1
//@[9:44) [BCP134 (Error)] Scope "module" is not valid for this module. Permitted scopes: "managementGroup". (CodeDescription: none) |managementGroupModuleDuplicateName1|
}

