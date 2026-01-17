// Bicep deployment for Bicep Extension Host on Azure Container Apps
// This deploys the Extension Host web application that downloads OCI artifacts
// and starts gRPC servers for Bicep extensions.

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

@description('The Azure Container Registry server URL')
param acrServer string

@description('Enable system-assigned managed identity for ACR pull')
param useSystemAssignedIdentity bool = true

@description('Extensions configuration as JSON array')
param extensionsConfigJson string = '[]'

@description('Extension storage path inside the container')
param extensionStoragePath string = '/app/extensions'

@description('CPU cores for the container')
param cpuCores string = '0.5'

@description('Memory for the container')
param memory string = '1Gi'

@description('Minimum number of replicas')
@minValue(0)
@maxValue(25)
param minReplicas int = 1

@description('Maximum number of replicas')
@minValue(1)
@maxValue(25)
param maxReplicas int = 3

@description('Tags to apply to all resources')
param tags object = {}

// ============================================================================
// Variables
// ============================================================================

var resourceToken = toLower(uniqueString(subscription().id, resourceGroup().id, location))
var abbrs = loadJsonContent('../abbreviations.json')

var environmentName = '${abbrs.appManagedEnvironments}${namePrefix}-${resourceToken}'
var containerAppName = '${abbrs.appContainerApps}${namePrefix}-${resourceToken}'
var logAnalyticsName = '${abbrs.operationalInsightsWorkspaces}${namePrefix}-${resourceToken}'
var userAssignedIdentityName = '${abbrs.managedIdentityUserAssignedIdentities}${namePrefix}-${resourceToken}'

// ============================================================================
// Resources
// ============================================================================

// Log Analytics Workspace for Container Apps Environment
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

// User-assigned managed identity for ACR pull
resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: userAssignedIdentityName
  location: location
  tags: tags
}

// Container Apps Environment
resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2025-01-01' = {
  name: environmentName
  location: location
  tags: tags
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
}

// Container App - Bicep Extension Host
resource containerApp 'Microsoft.App/containerApps@2025-01-01' = {
  name: containerAppName
  location: location
  tags: tags
  identity: useSystemAssignedIdentity ? {
    type: 'SystemAssigned,UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentity.id}': {}
    }
  } : {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentity.id}': {}
    }
  }
  properties: {
    environmentId: containerAppsEnvironment.id
    workloadProfileName: 'Consumption'
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
        allowInsecure: false
      }
      // No registries config needed - ACR allows anonymous pull
    }
    template: {
      containers: [
        {
          name: 'extension-host'
          image: containerImage
          resources: {
            cpu: json(cpuCores)
            memory: memory
          }
          env: [
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Production'
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
          probes: [
            {
              type: 'Liveness'
              httpGet: {
                path: '/health'
                port: 8080
                scheme: 'HTTP'
              }
              initialDelaySeconds: 30
              periodSeconds: 30
              failureThreshold: 3
            }
            {
              type: 'Readiness'
              httpGet: {
                path: '/health'
                port: 8080
                scheme: 'HTTP'
              }
              initialDelaySeconds: 10
              periodSeconds: 10
              failureThreshold: 3
            }
            {
              type: 'Startup'
              httpGet: {
                path: '/health'
                port: 8080
                scheme: 'HTTP'
              }
              initialDelaySeconds: 5
              periodSeconds: 5
              failureThreshold: 30
            }
          ]
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
        ]
      }
    }
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('The name of the Container App')
output containerAppName string = containerApp.name

@description('The FQDN of the Container App')
output containerAppFqdn string = containerApp.properties.configuration.ingress.fqdn

@description('The URL of the Container App')
output containerAppUrl string = 'https://${containerApp.properties.configuration.ingress.fqdn}'

@description('The name of the Container Apps Environment')
output environmentName string = containerAppsEnvironment.name

@description('The name of the Log Analytics Workspace')
output logAnalyticsWorkspaceName string = logAnalytics.name

@description('The principal ID of the user-assigned managed identity')
output userAssignedIdentityPrincipalId string = userAssignedIdentity.properties.principalId

@description('The client ID of the user-assigned managed identity')
output userAssignedIdentityClientId string = userAssignedIdentity.properties.clientId

@description('The resource ID of the user-assigned managed identity')
output userAssignedIdentityId string = userAssignedIdentity.id
