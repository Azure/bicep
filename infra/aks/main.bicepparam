using 'main.bicep'

// Required parameters
param namePrefix = 'bicepext'

// Optional parameters with defaults
param location = 'eastus'
param nodeVmSize = 'Standard_B2s' // Small VM for dev/test
param nodeCount = 1
param kubernetesVersion = '1.34'

// Tags
param tags = {
  application: 'bicep-extension-host'
  environment: 'production'
  hostingOption: 'aks'
}
