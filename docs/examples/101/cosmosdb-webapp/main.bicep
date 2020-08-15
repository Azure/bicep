parameter applicationName string = 'to-do-app${uniqueString(resourceGroup().id)}'
parameter location string = resourceGroup().location

parameter appServicePlanTier string {
    defaultValue: 'F1'
    allowedValues: [
      'F1'
      'D1'
      'B1'
      'B2'
      'B3'
      'S1'
      'S2'
      'S3'
      'P1'
      'P2'
      'P3'
      'P4'
    ]
}

parameter appServicePlanInstances int {
  defaultValue: 1
  minValue: 1
  maxValue: 3
}

parameter repositoryUrl string = 'https://github.com/Azure-Samples/cosmos-dotnet-core-todo-app.git'
parameter branch string = 'master'

parameter databaseName string = 'Tasks'
parameter containerName string = 'Items'

variable cosmosAccountName = toLower(applicationName)
variable websiteName = applicationName // why not just use the param directly?
variable hostingPlanName = applicationName // why not just use the param directly?

resource cosmos 'Microsoft.DocumentDB/databaseAccounts@2020-04-01' = {
  name: cosmosAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
       {
         locationName: location
         failoverPriority: 0
         isZoneRedundant: false
       }
    ]
    databaseAccountOfferType: 'Standard'
  }
}

resource farm 'Microsoft.Web/serverFarms@2019-08-01' = {
  // dependsOn: resourceId('Microsoft.DocumentDB/databaseAccounts', cosmosAccountName)
  name: hostingPlanName
  location: location
  sku: {
    name: appServicePlanTier
    capacity: appServicePlanInstances
  }
  properties: {
    name: '${hostingPlanName}' // just hostingPlanName results in an error
  }
}

resource website 'Microsoft.Web/sites@2019-08-01' = {
  // dependsOn: resourceId('Microsoft.Web/serverfarms', hostingPlanName) 
  name: websiteName
  location: location
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', hostingPlanName)
    siteConfig: {
      appSettings: [
        {
          name: 'CosmosDb:Account'
          // value: 
        }
      ]
    }
  }
}