@description('Location for all resources.')
param location string

@description('Base name that will appear for all resources.')
param baseName string = 'adecosmosapp2'

@description('Three letter environment abbreviation to denote environment that will appear in all resource names')
param environmentName string = 'cicd'

@description('App Service Plan Sku')
param appServicePlanSKU string = 'D1'

@description('Resource Group Log Analytics Workspace is in')
param logAnalyticsResourceGroup string

@description('Log Analytics Workspace Name')
param logAnalyticsWorkspace string

@description('Resource Group CosmosDB is in')
param cosmosDBResourceGroup string

@description('CosmosDB Name')
param cosmosDBName string

@description('Dev Center Project Name')
param devCenterProjectName string = ''

@description('Name for the Azure Deployment Environment')
param adeName string = ''

var regionReference = {
  centralus: 'cus'
  eastus: 'eus'
  westus: 'wus'
  westus2: 'wus2'
}
var language = 'Bicep'
var nameSuffix = (empty(adeName)
  ? toLower('${baseName}-${environmentName}-${regionReference[location]}')
  : '${devCenterProjectName}-${adeName}')

module userAssignedIdentityModule './nested_userAssignedIdentityModule.bicep' = {
  name: 'userAssignedIdentityModule'
  params: {
    location: location
    userIdentityName: nameSuffix
  }
}

module appServicePlanModule './nested_appServicePlanModule.bicep' = {
  name: 'appServicePlanModule'
  params: {
    location: location
    appServicePlanName: nameSuffix
    language: language
    appServicePlanSKU: appServicePlanSKU
    appServiceKind: 'linux'
  }
}

module appServiceModule './nested_appServiceModule.bicep' = {
  name: 'appServiceModule'
  params: {
    location: location
    appServicePlanID: reference(appServicePlanModule.id, '2022-09-01').outputs.appServicePlanID.value
//@[53:55) [BCP053 (Error)] The type "module" does not contain property "id". Available properties include "identity", "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |id|
    appServiceName: nameSuffix
    principalId: reference(userAssignedIdentityModule.id, '2022-09-01').outputs.userIdentityResourceId.value
//@[54:56) [BCP053 (Error)] The type "module" does not contain property "id". Available properties include "identity", "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |id|
    appSettingsArray: [
      {
        name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
        value: reference(appInsightsModule.id, '2022-09-01').outputs.appInsightsInstrumentationKey.value
//@[43:45) [BCP053 (Error)] The type "module" does not contain property "id". Available properties include "identity", "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |id|
      }
      {
        name: 'CosmosDb:Account'
        value: 'https://${cosmosDBName}.documents.azure.com:443/'
      }
      {
        name: 'CosmosDb:DatabaseName'
        value: 'Tasks'
      }
      {
        name: 'CosmosDb:ContainerName'
        value: 'Item'
      }
      {
        name: 'WEBSITE_RUN_FROM_PACKAGE'
        value: '1'
      }
      {
        name: 'SCM_DO_BUILD_DURING_DEPLOYMENT'
        value: 'true'
      }
      {
        name: 'ApplicationInsightsAgent_EXTENSION_VERSION'
        value: '~2'
      }
    ]
  }
}

module appInsightsModule './nested_appInsightsModule.bicep' = {
  name: 'appInsightsModule'
  params: {
    location: location
    appInsightsName: nameSuffix
    logAnalyticsWorkspaceID: extensionResourceId(
      '/subscriptions/${subscription().subscriptionId}/resourceGroups/${logAnalyticsResourceGroup}',
      'Microsoft.OperationalInsights/workspaces',
      logAnalyticsWorkspace
    )
    language: language
  }
}

module cosmosRBACModule './nested_cosmosRBACModule.bicep' = {
  name: 'cosmosRBACModule'
  scope: resourceGroup(cosmosDBResourceGroup)
  params: {
    databaseAccountName: cosmosDBName
    databaseAccountResourceGroup: cosmosDBResourceGroup
    principalId: reference(appServiceModule.id, '2022-09-01').outputs.appServiceManagedIdentity.value
//@[44:46) [BCP053 (Error)] The type "module" does not contain property "id". Available properties include "identity", "name", "outputs". (bicep https://aka.ms/bicep/core-diagnostics#BCP053) |id|
  }
}

