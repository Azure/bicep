using 'main.bicep'

// Required parameters
param namePrefix = 'bicepext'
param containerImage = 'ligaracr.azurecr.io/bicep-extension-host:latest'

// Optional parameters with defaults
param location = 'eastus'
param extensionStoragePath = '/app/extensions'
param appServicePlanSku = 'B1' // Basic tier (cheapest with always-on)

// Extensions configuration - JSON array of extension configs
// Example: '[{"Name":"kubernetes","RegistryUri":"https://myacr.azurecr.io","Repository":"bicep/extensions/kubernetes","Tag":"1.0.0"}]'
param extensionsConfigJson = '[]'

// Tags
param tags = {
  application: 'bicep-extension-host'
  environment: 'production'
  hostingOption: 'appservice'
}
