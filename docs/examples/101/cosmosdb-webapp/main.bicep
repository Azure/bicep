param applicationName string {
  default: 'to-do-app${uniqueString(resourceGroup().id)}'
  maxLength: 30
}
param location string = resourceGroup().location

param appServicePlanTier string {
    default: 'F1'
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

param appServicePlanInstances int {
  default: 1
  minValue: 1
  maxValue: 3
}

param repositoryUrl string = 'https://github.com/Azure-Samples/cosmos-dotnet-core-todo-app.git'
param branch string = 'master'

param databaseName string = 'Tasks'
param containerName string = 'Items'

var cosmosAccountName = toLower(applicationName)
var websiteName = applicationName // why not just use the param directly?
var hostingPlanName = applicationName // why not just use the param directly?

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
  name: hostingPlanName
  location: location
  sku: {
    name: appServicePlanTier
    capacity: appServicePlanInstances
  }
  properties: {
    name: hostingPlanName
  }
}

resource website 'Microsoft.Web/sites@2019-08-01' = {
  name: websiteName
  location: location
  properties: {
    serverFarmId: resourceId('Microsoft.Web/serverfarms', hostingPlanName)
    siteConfig: {
      appSettings: [
        {
          name: 'CosmosDb:Account'
          value: cosmos.properties.documentEndpoint 
        }
        {
          name: 'CosmosDb:Key'
          value: listKeys(cosmos.id, cosmos.apiVersion).primaryMasterKey
        }
        {
          name: 'CosmosDb:DatabaseName'
          value: databaseName
        }
        {
          name: 'CosmosDb:ContainerName'
          value: containerName
        }
      ]
    }
  }
}

resource srcCtrl 'Microsoft.Web/sites/sourcecontrols@2019-08-01' = {
  name: '${website.name}/web'
  location: location
  properties: {
    repoUrl: repositoryUrl
    branch: branch
    isManualIntegration: true
  }
}