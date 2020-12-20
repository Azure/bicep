targetScope = 'tenant'

module tenantModule 'modules/tenant.bicep' = {
  name: 'tenant'
}

module managementGroupModule 'modules/managementGroup.bicep' = {
  name: 'managementGroup'
  scope: managementGroup('MG')
}

module subscriptionModule 'modules/subscription.bicep' = {
  name: 'subscription'
  scope: subscription('SUB')
}

resource errorMaker //Incomplete resource for Integration Tests stub - Valid test must pass without warnings, while Invalid ones must not create template and raise an error. Hence this incomplete declaration.