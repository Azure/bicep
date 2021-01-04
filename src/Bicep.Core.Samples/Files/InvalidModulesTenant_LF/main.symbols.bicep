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

