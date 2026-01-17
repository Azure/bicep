using 'main.bicep'

// Required parameters
param namePrefix = 'bicepext'
param containerImage = 'ligaracr.azurecr.io/bicep-extension-host:latest'
param acrServer = 'ligaracr.azurecr.io'

// Optional parameters with defaults
param location = 'eastus'
param useSystemAssignedIdentity = true
param extensionStoragePath = '/app/extensions'
param cpuCores = '0.5'
param memory = '1Gi'
param minReplicas = 1
param maxReplicas = 3

// Extensions configuration - JSON array of extension configs
// Example: '[{"Name":"kubernetes","RegistryUri":"https://myacr.azurecr.io","Repository":"bicep/extensions/kubernetes","Tag":"1.0.0"}]'
param extensionsConfigJson = '[]'

// Tags
param tags = {
  application: 'bicep-extension-host'
  environment: 'production'
  hostingOption: 'aca'
}
