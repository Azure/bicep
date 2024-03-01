@description('Name for the App Service Plan')
param appServicePlanName string

@description('Location for resource.')
param location string

@description('What language was used to deploy this resource')
param language string

@description('AppService Plan Sku')
@allowed([
  'B1'
  'B2'
  'B3'
  'D1'
  'F1'
  'FREE'
  'I1'
  'I1v2'
  'I2'
  'I2v2'
  'I3'
  'I3v2'
  'P1V2'
  'P1V3'
  'P2V2'
  'P2V3'
  'P3V2'
  'P3V3'
  'S1'
  'S2'
  'S3'
  'SHARED'
  'WS1'
  'WS2'
  'WS3'
])
param appServicePlanSKU string = 'D1'

@description('AppService Plan Kind')
@allowed([
  'windows'
  'linux'
  'windowscontainer'
])
param appServiceKind string = 'windows'

resource asp_appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: toLower('asp-${appServicePlanName}')
  location: location
  kind: appServiceKind
  sku: {
    name: appServicePlanSKU
  }
  tags: {
    Language: language
  }
  properties: {}
}

output appServicePlanID string = asp_appServicePlan.id

