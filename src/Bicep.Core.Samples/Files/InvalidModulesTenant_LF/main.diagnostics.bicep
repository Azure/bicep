targetScope = 'tenant'

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP121 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'tenantModuleDuplicateName'|
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP121 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'tenantModuleDuplicateName'|
  scope: tenant()
}

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
  name: 'tenantModuleDuplicateName'
//@[8:35) [BCP121 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2", "tenantModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'tenantModuleDuplicateName'|
}

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP121 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'managementGroupModuleDuplicateName'|
  scope: managementGroup('MG')
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP121 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'managementGroupModuleDuplicateName'|
  scope: managementGroup('MG')
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionModuleDuplicateName'
//@[8:41) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionModuleDuplicateName'|
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

