param hostingPlanName string
param location string = resourceGroup().location

var siteName_var = 'ExampleSite${uniqueString(resourceGroup().id)}'

resource hostingPlanName_resource 'Microsoft.Web/serverfarms@2020-06-01' = {
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

resource siteName 'Microsoft.Web/sites@2020-06-01' = {
  name: siteName_var
  location: location
  properties: {
    serverFarmId: hostingPlanName
  }
  dependsOn: [
    hostingPlanName_resource
  ]
}

resource siteLock 'Microsoft.Authorization/locks@2016-09-01' = {
  scope: siteName
  name: 'siteLock'
  properties: {
    level: 'CanNotDelete'
    notes: 'Site should not be deleted.'
  }
  dependsOn: [
    siteName
  ]
}
