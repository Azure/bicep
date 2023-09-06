targetScope = 'managementGroup'

module managementGroupModuleDuplicateName1 'modules/managementGroup.bicep' = {
//@[7:42) Module managementGroupModuleDuplicateName1. Type: module. Declaration start char: 0, length: 152
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup()
}

module managementGroupModuleDuplicateName2 'modules/managementGroup.bicep' = {
//@[7:42) Module managementGroupModuleDuplicateName2. Type: module. Declaration start char: 0, length: 152
  name: 'managementGroupModuleDuplicateName'
  scope: managementGroup()
}

module managementGroupModuleDuplicateName3 'modules/managementGroup.bicep' = {
//@[7:42) Module managementGroupModuleDuplicateName3. Type: module. Declaration start char: 0, length: 125
  name: 'managementGroupModuleDuplicateName'
}

module subscriptionModuleDuplicateName1 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName1. Type: module. Declaration start char: 0, length: 172
  name: 'subscriptionDuplicateName'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module subscriptionModuleDuplicateName2 'modules/subscription.bicep' = {
//@[7:39) Module subscriptionModuleDuplicateName2. Type: module. Declaration start char: 0, length: 172
  name: 'subscriptionDuplicateName'
  scope: subscription('c56ffff6-0806-4a98-83fb-17aed775d6e4')
}

module tenantModuleDuplicateName1 'modules/tenant.bicep' = {
//@[7:33) Module tenantModuleDuplicateName1. Type: module. Declaration start char: 0, length: 110
  name: 'tenantDuplicateName'
  scope: tenant()
}

module tenantModuleDuplicateName2 'modules/tenant.bicep' = {
//@[7:33) Module tenantModuleDuplicateName2. Type: module. Declaration start char: 0, length: 110
  name: 'tenantDuplicateName'
  scope: tenant()
}

