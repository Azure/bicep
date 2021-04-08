targetScope = 'tenant'

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[7:33) Module tenantModuleDuplicateName1. Type: module. Declaration start char: 0, length: 116
  name: 'tenantModuleDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
//@[7:33) Module tenantModuleDuplicateName2. Type: module. Declaration start char: 0, length: 116
  name: 'tenantModuleDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName3 'modules/tenant.bicep' = {
//@[7:33) Module tenantModuleDuplicateName3. Type: module. Declaration start char: 0, length: 98
  name: 'tenantModuleDuplicateName'
}

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[7:42) Module managementGroupModuleDuplicateName1. Type: module. Declaration start char: 0, length: 156
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup('MG')
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[7:42) Module managementGroupModuleDuplicateName2. Type: module. Declaration start char: 0, length: 156
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup('MG')
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName1. Type: module. Declaration start char: 0, length: 178
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName2. Type: module. Declaration start char: 0, length: 178
  name: 'subscriptionModuleDuplicateName'
  scope: subscription('1ad827ac-2669-4c2f-9970-282b93c3c550')
}

module managementGroupModules 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[70:72) Local mg. Type: any. Declaration start char: 70, length: 2
//@[74:75) Local i. Type: int. Declaration start char: 74, length: 1
//@[7:29) Module managementGroupModules. Type: module[]. Declaration start char: 0, length: 137
  name: 'dep-${mg}'
  scope: managementGroup(mg)
}]

module cannotUseModuleCollectionAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[80:82) Local mg. Type: any. Declaration start char: 80, length: 2
//@[84:85) Local i. Type: int. Declaration start char: 84, length: 1
//@[7:39) Module cannotUseModuleCollectionAsScope. Type: module[]. Declaration start char: 0, length: 150
  name: 'dep-${mg}'
  scope: managementGroupModules
}]

module cannotUseSingleModuleAsScope 'modules/managementGroup.bicep' = [for (mg, i) in []: {
//@[76:78) Local mg. Type: any. Declaration start char: 76, length: 2
//@[80:81) Local i. Type: int. Declaration start char: 80, length: 1
//@[7:35) Module cannotUseSingleModuleAsScope. Type: module[]. Declaration start char: 0, length: 149
  name: 'dep-${mg}'
  scope: managementGroupModules[i]
}]

module cannotUseSingleModuleAsScope2 'modules/managementGroup.bicep' = {
//@[7:36) Module cannotUseSingleModuleAsScope2. Type: module. Declaration start char: 0, length: 134
  name: 'test'
  scope: managementGroupModuleDuplicateName1
}

