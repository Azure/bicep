// Bicep deployment for Bicep Extension Host on Azure App Service (Container)
// This deploys the Extension Host as a container on App Service.
// NOTE: App Service has sandbox limitations that may prevent spawning child processes.

targetScope = 'resourceGroup'

// ============================================================================
// Parameters
// ============================================================================

@description('The location for all resources')
param location string = resourceGroup().location

@description('The name prefix for all resources')
@minLength(3)
@maxLength(11)
param namePrefix string = 'bicepext'

@description('The container image to deploy (e.g., myacr.azurecr.io/bicep-extension-host:latest)')
param containerImage string

@description('Extensions configuration as JSON array')
param extensionsConfigJson string = '[]'

@description('Extension storage path inside the container')
param extensionStoragePath string = '/app/extensions'

@description('App Service Plan SKU')
@allowed(['B1', 'B2', 'B3', 'S1', 'S2', 'S3', 'P1v2', 'P2v2', 'P3v2', 'P1v3', 'P2v3', 'P3v3'])
param appServicePlanSku string = 'B1'

@description('Tags to apply to all resources')
param tags object = {}

// ============================================================================
// Variables
// ============================================================================

var resourceToken = toLower(uniqueString(subscription().id, resourceGroup().id, location))
var abbrs = loadJsonContent('../abbreviations.json')

var appServicePlanName = '${abbrs.webServerFarms}${namePrefix}-${resourceToken}'
var webAppName = '${abbrs.webSitesAppService}${namePrefix}-${resourceToken}'
var logAnalyticsName = '${abbrs.operationalInsightsWorkspaces}${namePrefix}-${resourceToken}'

// ============================================================================
// Resources
// ============================================================================

// Log Analytics Workspace for diagnostics
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: logAnalyticsName
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// App Service Plan (Linux for containers)
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'linux'
  sku: {
    name: appServicePlanSku
  }
  properties: {
    reserved: true // Required for Linux
  }
}

// Web App (Container)
resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux,container'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOCKER|${containerImage}'
      alwaysOn: true
      http20Enabled: true
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'DOCKER_ENABLE_CI'
          value: 'false'
        }
        {
          name: 'WEBSITES_PORT'
          value: '8080'
        }
        {
          name: 'ExtensionHost__ExtensionStoragePath'
          value: extensionStoragePath
        }
        {
          name: 'ExtensionHost__Extensions'
          value: extensionsConfigJson
        }
      ]
      healthCheckPath: '/health'
    }
  }
}

// Diagnostic settings
resource webAppDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: 'diag-${webAppName}'
  scope: webApp
  properties: {
    workspaceId: logAnalytics.id
    logs: [
      {
        category: 'AppServiceHTTPLogs'
        enabled: true
      }
      {
        category: 'AppServiceConsoleLogs'
        enabled: true
      }
      {
        category: 'AppServiceAppLogs'
        enabled: true
      }
      {
        category: 'AppServicePlatformLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('The name of the Web App')
output webAppName string = webApp.name

@description('The default hostname of the Web App')
output webAppHostname string = webApp.properties.defaultHostName

@description('The URL of the Web App')
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'

@description('The name of the App Service Plan')
output appServicePlanName string = appServicePlan.name

@description('The name of the Log Analytics Workspace')
output logAnalyticsWorkspaceName string = logAnalytics.name
