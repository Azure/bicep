@description('Name for the App Service')
param appServiceName string

@description('Location for resource.')
param location string

@description('Resource ID of the App Service Plan')
param appServicePlanID string

@description('User Asisgned Identity for App Service')
param principalId string = ''

@description('App Settings for the Application')
param appSettingsArray array = []

resource app_appService 'Microsoft.Web/sites@2022-09-01' = {
  name: toLower('app-${appServiceName}')
  location: location
  identity: (empty(principalId)
    ? {
        type: 'SystemAssigned'
      }
    : {
        type: 'SystemAssigned, UserAssigned'
        userAssignedIdentities: {
          '${principalId}': {}
        }
      })
  tags: {
    displayName: 'Website'
  }
  properties: {
    serverFarmId: appServicePlanID
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      appSettings: appSettingsArray
    }
  }
}

output appServiceManagedIdentity string = reference(app_appService.id, '2022-09-01', 'full').identity.principalId
//@[42:92) [use-resource-symbol-reference (Warning)] Use a resource reference instead of invoking function "reference". This simplifies the syntax and allows Bicep to better understand your deployment dependency graph. (bicep core linter https://aka.ms/bicep/linter-diagnostics#use-resource-symbol-reference) |reference(app_appService.id, '2022-09-01', 'full')|
output appServiceName string = toLower('app-${appServiceName}')

