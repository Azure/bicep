targetScope = 'managementGroup'

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP121 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2", "managementGroupModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'managementGroupModuleDuplicateName'|
  scope: managementGroup()
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP121 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2", "managementGroupModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'managementGroupModuleDuplicateName'|
  scope: managementGroup()
}

module managementGroupModuleDuplicateName3 'modules/managementGroup.bicep' = {
  name: 'managementGroupModuleDuplicateName'
//@[8:44) [BCP121 (Error)] Modules: "managementGroupModuleDuplicateName1", "managementGroupModuleDuplicateName2", "managementGroupModuleDuplicateName3" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'managementGroupModuleDuplicateName'|
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
  name: 'subscriptionDuplicateName'
//@[8:35) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionDuplicateName'|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
  name: 'subscriptionDuplicateName'
//@[8:35) [BCP121 (Error)] Modules: "subscriptionModuleDuplicateName1", "subscriptionModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'subscriptionDuplicateName'|
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
  name: 'tenantDuplicateName'
//@[8:29) [BCP121 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'tenantDuplicateName'|
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
  name: 'tenantDuplicateName'
//@[8:29) [BCP121 (Error)] Modules: "tenantModuleDuplicateName1", "tenantModuleDuplicateName2" are defined with this same name and this same scope in a file. Rename them or split into different modules. |'tenantDuplicateName'|
  scope: tenant()
}

