targetScope = 'tenant'

module tenantModule 'modules/tenant.bicep' = {
//@[7:19) Module tenantModule. Type: module. Declaration start char: 0, length: 65
  name: 'tenant'
}

module managementGroupModule 'modules/managementGroup.bicep' = {
//@[7:28) Module managementGroupModule. Type: module. Declaration start char: 0, length: 123
  name: 'managementGroup'
  scope: managementGroup('MG')
}

module subscriptionModule 'modules/subscription.bicep' = {
//@[7:25) Module subscriptionModule. Type: module. Declaration start char: 0, length: 112
  name: 'subscription'
  scope: subscription('SUB')
}

resource errorMaker //Incomplete resource for Integration Tests stub - Valid test must pass without warnings, while Invalid ones must not create template and raise an error. Hence this incomplete declaration.
//@[9:19) Resource errorMaker. Type: error. Declaration start char: 0, length: 208
