param hostingPlanName string
param location string = resourceGroup().location

var siteName = 'ExampleSite${uniqueString(resourceGroup().id)}'

resource hostingPlan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: hostingPlanName
  location: location
  sku: {
    tier: 'Free'
    name: 'f1'
    capacity: 0
  }
  properties: {
    targetWorkerCount: 1
  }
}

resource site 'Microsoft.Web/sites@2020-06-01' = {
  name: siteName
  location: location
  properties: {
    serverFarmId: hostingPlanName
  }
  dependsOn: [
    hostingPlan
  ]
}

resource siteLock 'Microsoft.Authorization/locks@2016-09-01' = {
  scope: site
  name: 'siteLock'
  properties: {
    level: 'CanNotDelete'
    notes: 'Site should not be deleted.'
  }
}
