// ****************************************
// Azure Bicep main template
// This bicep template demonstrates modular way of deploying Azure API Management
// This example used modules for Groups, Name Value pairs, Users and so on and also illustrates using modules
// ****************************************

targetScope = 'resourceGroup'
param apimInstanceName string = 'PGAPIM-${uniqueString(resourceGroup().id)}' //will add 13 characters to the name

//be default we keep the appinsights name same as APIM instance
param appInsightsName string = apimInstanceName

param resourceTags object = {
  'ProjectType': 'API Management'
  'Purpose': 'Demo'
}

@minLength(1)
@description('The email address of the owner of the service')
param publisherEmail string

@minLength(1)
@description('The name of the owner of the service')
param publisherName string

@allowed([
  'Consumption'
  'Developer'
  'Basic'
  'Standard'
  'Premium'
])
@description('The pricing tier of this API Management service')
param sku string = 'Developer'

@allowed([
  1
  2
])
@description('The instance size of this API Management service.')
param skuCount int = 1

@description('Location for all resources.')
param location string = resourceGroup().location


resource applicationInsights 'Microsoft.Insights/components@2015-05-01' = {
  name: appInsightsName
  location: location
  tags: resourceTags
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource apiManagement 'Microsoft.ApiManagement/service@2019-01-01' = {
  name: apimInstanceName
  location: location
  tags: resourceTags
  sku: {
    name: sku
    capacity: skuCount
  }
  properties: {
    publisherEmail: publisherEmail
    publisherName: publisherName
  }
}

resource apiManagement_logger 'Microsoft.ApiManagement/service/loggers@2019-01-01' = {
  name: '${apiManagement.name}/${appInsightsName}'
  properties: {
    loggerType: 'applicationInsights'
    description: 'Logger resources to APIM'
    credentials: {
      instrumentationKey: applicationInsights.properties['InstrumentationKey']
    }
  }
}

resource apimInstanceName_diagnostics 'Microsoft.ApiManagement/service/diagnostics@2020-06-01-preview' = {
  name: '${apiManagement.name}/applicationinsights'
  dependsOn: [
    apiManagement_logger
  ]
  properties: {
    loggerId: apiManagement_logger.id
    alwaysLog: 'allErrors'
    logClientIp: true
    sampling: {
      percentage: 100
      samplingType: 'fixed'
    }
  }
}

// ****************************************
// Add optional modules as and when needed
// ****************************************
//optional modules

//Include Group modules
module apimGroup './groups.bicep' = {
  params: { 
    apimInstanceName: apiManagement.name
  }
  name:'apimGroups'
  dependsOn:[
    apiManagement
  ]
}

//Include users modules
module apimUsers './users.bicep' = {
  params: { 
    apimInstanceName: apiManagement.name
  }
  name:'apimUsers'
  dependsOn:[
    apiManagement
  ]
}

//include Name value pair modules
module apimNVPairs './NameValues.bicep' = {
  params: {
    apimInstanceName: apiManagement.name
  }
  name: 'apimNameValuePairs'
  dependsOn:[
    apiManagement
  ]
}
output appInsightsInstrumentationKey string = applicationInsights.properties.InstrumentationKey
output apimURL string = apiManagement.properties.portalUrl
